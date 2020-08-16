import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { ApiService } from '../../services/api.service';
import { SpotifyService } from '../../services/spotify.service';
import { LoggerService, LogType } from '../../services/logger.service';
import { LoaderService } from '../../services/loader.service';
import { SpotifyUser } from '../../models/spotify-user';

@Component({
  selector: 'sfm-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent implements OnInit {

  public formGroup: FormGroup;

  constructor(
    private api: ApiService,
    private spotify: SpotifyService,
    private logger: LoggerService,
    private loader: LoaderService,
  ) {
    this.formGroup = new FormGroup({
      address: new FormGroup({
        street: new FormControl(null, [Validators.required]),
        number: new FormControl(null, [Validators.required]),
        postcode: new FormControl(null, [Validators.required]),
        city: new FormControl(null, [Validators.required])
      }),
      plan: new FormGroup({})
    });
  }

  public ngOnInit(): void {
  }

  public async proceedPayment(): Promise<void> {
    try {
      this.loader.show();
      const user: SpotifyUser = await this.spotify.getUser();
      console.log(user);
      const url: { link: string, token: string } = await this.api.makeRequest<{ link: string, token }>('POST', `api/paypal/${user.id}/Subscribe`);
      const win = window.open(url.link);
      const i = setInterval(async () => {
        try {
          if (win?.closed) {
            clearInterval(i);
            await this.api.makeRequest('POST', `api/paypal/ActivateSubscription/${user.id}/${url.token}`);

            this.logger.log(LogType.Success, 'Subscription added');
            this.loader.hide();
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

}
