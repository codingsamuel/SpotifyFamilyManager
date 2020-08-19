import { Component, OnInit } from '@angular/core';
import { SpotifyService } from '../../services/spotify.service';
import { SpotifyUser } from '../../models/spotify-user';

@Component({
  selector: 'sfm-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  public user: SpotifyUser;

  constructor(
    public spotify: SpotifyService
  ) {
  }

  public ngOnInit(): void {
    // this.spotify.getUser().then();
    this.loadData().then();
  }

  get nextPayment() {
    const lastPayment = new Date(this.spotify.subscription.lastPayment);
    const interval = this.spotify.subscription.paymentInterval;

    lastPayment.setMonth(lastPayment.getMonth() + interval);
    return lastPayment;
  }

  public async loadData(): Promise<void> {
    this.user = await this.spotify.getUser();
    console.log(await this.spotify.getSubscription());
  }

}
