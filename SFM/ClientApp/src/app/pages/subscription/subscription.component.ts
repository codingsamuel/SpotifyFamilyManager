import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { SpotifyService } from '../../services/spotify.service';
import { LoggerService, LogType } from '../../services/logger.service';
import { LoaderService } from '../../services/loader.service';
import { SpotifyUser } from '../../models/spotify-user';
import { IPlan } from '../../models/plan';
import { ISubscriptionResponse } from '../../models/subscription-response';
import { ISubscription } from '../../models/subscription';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'sfm-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent implements OnInit {

  public formGroup: FormGroup;
  public subscription: ISubscription;
  public plans: IPlan[] = [
    {
      id: 1,
      icon: 'elderly',
      title: 'sfm.subscription.plans.alman',
      subtitle: 'sfm.subscription.plans.alman.desc',
      price: 2.50,
      tags: [
        'monthly'
      ],
      interval: 1
    },
    {
      id: 2,
      icon: 'accessible',
      title: 'sfm.subscription.plans.lazy-shit',
      subtitle: 'sfm.subscription.plans.lazy-shit.desc',
      price: 10,
      tags: [
        'all 4 months'
      ],
      interval: 4
    }
  ];
  public activePlanId: number;
  private user: SpotifyUser;

  constructor(
    private api: ApiService,
    private spotify: SpotifyService,
    private logger: LoggerService,
    private loader: LoaderService,
    private translate: TranslateService,
  ) {
    this.formGroup = new FormGroup({
      address: new FormGroup({
        street: new FormControl(null, [Validators.required]),
        number: new FormControl(null, [Validators.required]),
        postcode: new FormControl(null, [Validators.required]),
        city: new FormControl(null, [Validators.required]),
        state: new FormControl(null, [Validators.required])
      }),
      interval: new FormControl(1),
    });

    if (window.localStorage.getItem('subscription_cache')) {
      const data = JSON.parse(window.localStorage.getItem('subscription_cache'));
      this.formGroup.patchValue(data);
      this.activePlanId = this.plans.filter(p => p.interval === data.interval)[0]?.id;
    }
  }

  public ngOnInit(): void {
    this.loadData().then();
  }

  public async proceedPayment(): Promise<void> {
    try {
      this.loader.show();
      const url: ISubscriptionResponse = await this.api.makeRequest<ISubscriptionResponse>(
        'POST', `api/paypal/${this.user.id}/Subscribe`, this.formGroup.getRawValue());
      const win = window.open(url.link);
      const i = setInterval(async () => {
        try {
          if (win?.closed) {
            clearInterval(i);
            await this.api.makeRequest('POST', `api/paypal/ActivateSubscription/${this.user.id}/${url.token}`);
            this.logger.log(LogType.Success, this.translate.instant('sfm.subscription.added'));
            this.loader.hide();
            await this.loadData();
          }
        } catch (ex) {
          this.loader.hide();
          this.logger.log(LogType.Error, ex);
        }
      }, 1000);
    } catch (ex) {
      this.loader.hide();
      this.logger.log(LogType.Error, ex);
    }
  }

  public saveLocal(): void {
    window.localStorage.setItem('subscription_cache', JSON.stringify(this.formGroup.getRawValue()));
  }

  public selectPlan(plan: IPlan): void {
    this.formGroup.controls.interval.patchValue(plan.interval);
    this.activePlanId = plan.id;
  }

  private async loadData(): Promise<void> {
    const user: SpotifyUser = await this.spotify.getUser();
    this.user = user;
    this.subscription = await this.api.makeRequest('GET', `api/paypal/GetSubscription/${user.id}`);

    if (this.subscription.active) {
      this.formGroup.disable();
    }
  }

}
