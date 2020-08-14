import { Component } from '@angular/core';
import { NavGroup } from './models/nav-item';
import { Router } from '@angular/router';
import { SpotifyService } from './services/spotify.service';
import { ISpotifyUser } from './models/spotify-user';

@Component({
  selector: 'sfm-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  public navGroup: NavGroup[] = [
    {
      name: 'General',
      items: [
        {
          name: 'Login',
          icon: 'login',
          action: () => {
            this.router.navigate(['login']).then();
          },
        },
        {
          name: 'Home',
          icon: 'home',
          action: () => {
            this.router.navigate(['home']).then();
          },
          loggedIn: true
        },
        {
          name: 'Abonnement',
          icon: 'payment',
          action: () => {
            this.router.navigate(['subscription']).then();
          },
          loggedIn: true
        }
      ],
    },
    {
      name: 'Einstellungen',
      items: [
        {
          name: 'Sprache',
          icon: 'translate',
          action: () => {
            return;
          },
        },
      ]
    }
  ];

  constructor(
    private router: Router,
    private spotify: SpotifyService
  ) {
    this.spotify.getUser().then();
    this.spotify.userChange.subscribe((user: ISpotifyUser) => {
      this.navGroup = this.navGroup.filter(group => {
        group.items = group.items.filter(item => item.loggedIn);
        return group.items.length > 0;
      });
    });
  }


}