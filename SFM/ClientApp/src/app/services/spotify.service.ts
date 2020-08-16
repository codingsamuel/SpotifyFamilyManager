import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpHeaders } from '@angular/common/http';
import { ISpotifyToken } from '../models/spotfy-token.model';
import { ISpotifyUser, SpotifyUser } from '../models/spotify-user';
import { Subject } from 'rxjs';

const CLIENT_ID = '9239bb996ba8466ab4ac58bdff5f5466';
const CLIENT_SECRET = 'f9e980c5e4da4d55af66a3fe04ecb63d';
const REDIRECT_URI = 'https://localhost:5001/login';
const SCOPES = 'user-read-private user-read-email';

@Injectable({
  providedIn: 'root'
})
export class SpotifyService {

  public userChange: Subject<SpotifyUser>;
  private user: SpotifyUser;

  constructor(
    private api: ApiService
  ) {
    this.userChange = new Subject<SpotifyUser>();
    this.userChange.subscribe(user => {
      this.user = user;
    });
  }

  public login(): void {
    const win = window.open(`https://accounts.spotify.com/authorize?response_type=code&client_id=${CLIENT_ID}&scope=${SCOPES}&redirect_uri=${REDIRECT_URI}&show_dialog=true`);
    const i = setInterval(() => {
      if (win?.closed) {
        clearInterval(i);
      }
    }, 1000);
  }

  public async generateToken(code: string): Promise<ISpotifyToken> {
    return this.api.makeRequest<ISpotifyToken>('POST', `https://accounts.spotify.com/api/token`, {
      grant_type: 'authorization_code',
      code,
      redirect_uri: REDIRECT_URI,
      client_id: CLIENT_ID,
      client_secret: CLIENT_SECRET
    }, new HttpHeaders({'Content-Type': 'application/x-www-form-urlencoded'}), true)
      .then((token: ISpotifyToken) => {
        this.api.setToken(token);
        return token;
      });
  }

  public async refreshToken(): Promise<boolean> {
    if (this.api.getToken()) {
      return this.api.makeRequest<ISpotifyToken>('POST', `https://accounts.spotify.com/api/token`, {
        grant_type: 'refresh_token',
        refresh_token: this.api.getToken().refresh_token,
        client_id: CLIENT_ID,
        client_secret: CLIENT_SECRET
      }, new HttpHeaders({'Content-Type': 'application/x-www-form-urlencoded'}), true)
        .then((token: ISpotifyToken) => {
          this.api.setToken(token);
          return true;
        });
    }
    return false;
  }

  public async getUser(): Promise<SpotifyUser> {
    if (this.user) return this.user;
    return this.api.makeRequest<ISpotifyUser>('GET', 'https://api.spotify.com/v1/me', async () => {
      if (await this.refreshToken()) {
        await this.getUser();
      }
    }).then(async (user: ISpotifyUser) => {
      const spotifyUser = await this.api.makeRequest<SpotifyUser>('POST', `api/SpotifyUser`, new SpotifyUser(user));
      this.userChange.next(spotifyUser);
      return spotifyUser;
    });
  }

}
