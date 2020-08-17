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
    private spotify: SpotifyService
  ) {
  }

  public ngOnInit(): void {
    // this.spotify.getUser().then();
    this.loadData().then();
  }

  public async loadData(): Promise<void> {
    this.user = await this.spotify.getUser();
  }

}
