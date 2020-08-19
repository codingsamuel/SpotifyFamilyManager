import { Component, OnInit } from '@angular/core';
import { NavGroup } from './models/nav-item';
import { Router } from '@angular/router';
import { SpotifyService } from './services/spotify.service';
import { SpotifyUser } from './models/spotify-user';
import { LoaderService } from './services/loader.service';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'sfm-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  public navGroup: NavGroup[] = [];

  constructor(
    private router: Router,
    private spotify: SpotifyService,
    private translate: TranslateService,
    public loader: LoaderService,
  ) {
    this.translate.setDefaultLang('de-DE');
    this.translate.use('de-DE').subscribe(data => {
      this.navGroup = [
        {
          name: 'General',
          items: [
            {
              name: 'sfm.common.login',
              icon: 'login',
              action: async () => {
                await this.router.navigate(['login']).then();
              },
            },
            {
              name: 'sfm.common.home',
              icon: 'home',
              action: async () => {
                await this.router.navigate(['home']).then();
              },
              loggedIn: true
            },
            {
              name: 'sfm.common.subscription',
              icon: 'payment',
              action: async () => {
                await this.router.navigate(['subscription']).then();
              },
              loggedIn: true
            }
          ],
        },
        {
          name: 'Einstellungen',
          items: [
            {
              name: 'sfm.common.language',
              icon: 'translate',
              action: () => {
                return;
              },
            },
          ]
        }
      ];
    });
  }

  public ngOnInit(): void {
    this.loadUser().then();
  }

  private async loadUser(): Promise<void> {
    await this.spotify.getUser();
    this.spotify.userChange.subscribe((user: SpotifyUser) => {
      this.navGroup = this.navGroup.filter(group => {
        group.items = group.items.filter(item => item.loggedIn);
        return group.items.length > 0;
      });
    });
  }

}