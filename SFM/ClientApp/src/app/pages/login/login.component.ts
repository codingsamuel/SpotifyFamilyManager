import { Component, OnInit } from '@angular/core';
import { SpotifyService } from '../../services/spotify.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'sfm-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {

  constructor(
    private spotify: SpotifyService,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.route.queryParams.subscribe(async params => {
      if (params.code) {
        await this.spotify.generateToken(params.code);
        await this.router.navigate(['home']);
        window.close();
      }
    });
  }

  public ngOnInit(): void {

  }

  public login(): void {
    this.spotify.login();
  }

}
