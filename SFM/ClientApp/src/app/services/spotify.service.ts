import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { HttpHeaders } from '@angular/common/http';
import { ISpotifyToken } from '../models/spotfy-token.model';
import { ISpotifyUser, SpotifyUser } from '../models/spotify-user';
import { Subject } from 'rxjs';
import { ISubscription } from '../models/subscription';
import { ISpotifyConfig } from '../models/spotify-config';

const REDIRECT_URI = 'https://localhost:5001/login';
const SCOPES = 'user-read-private user-read-email';

@Injectable({
  providedIn: 'root'
})
export class SpotifyService {

  public user: SpotifyUser;
  public userChange: Subject<SpotifyUser>;

  public subscription: ISubscription;
  public subscriptionChange: Subject<ISubscription>;

  public spotifyConfig: ISpotifyConfig;

  constructor(
    private api: ApiService
  ) {
    this.userChange = new Subject<SpotifyUser>();
    this.subscriptionChange = new Subject<ISubscription>();

    this.api.makeRequest('GET', `api/config/GetSpotifyConfig`)
      .then((data: ISpotifyConfig) => {
        this.spotifyConfig = data;
        return data;
      });
  }

  public async login(): Promise<void> {
    const win = window.open(`https://accounts.spotify.com/authorize?response_type=code&client_id=${this.spotifyConfig.clientId}&scope=${SCOPES}&redirect_uri=${REDIRECT_URI}&show_dialog=true`);
    const i = setInterval(async () => {
      if (win?.closed) {
        clearInterval(i);
        await this.getUser();
      }
    }, 1000);
  }

  public async generateToken(code: string): Promise<ISpotifyToken> {
    return this.api.makeRequest<ISpotifyToken>('POST', `https://accounts.spotify.com/api/token`, {
      grant_type: 'authorization_code',
      code,
      redirect_uri: REDIRECT_URI,
      client_id: this.spotifyConfig.clientId,
      client_secret: this.spotifyConfig.clientSecret
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
        client_id: this.spotifyConfig.clientId,
        client_secret: this.spotifyConfig.clientSecret
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
    return this.api.makeRequest<ISpotifyUser>('GET', 'https://api.spotify.com/v1/me').then(async (user: ISpotifyUser) => {
      const spotifyUser = await this.api.makeRequest<SpotifyUser>('POST', `api/SpotifyUser`, new SpotifyUser(user));
      this.user = spotifyUser;
      this.userChange.next(spotifyUser);
      return spotifyUser;
    })
      .catch(async (err) => {
        if (await this.refreshToken()) {
          await this.getUser();
        }
        return null;
      });
  }

  public async getSubscription(): Promise<ISubscription> {
    const subscription: ISubscription = await this.api.makeRequest<ISubscription>('GET', `api/PayPal/getSubscription/${this.user.id}`);
    this.subscriptionChange.next(subscription);
    this.subscription = subscription;
    return subscription;
  }

}
