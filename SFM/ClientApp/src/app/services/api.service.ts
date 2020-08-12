import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ISpotifyToken } from '../models/spotfy-token.model';

export type HttpMethod = 'get' | 'patch' | 'post' | 'delete';

@Injectable({
  providedIn: 'root'
})
export class ApiService {

  constructor(
    private http: HttpClient,
  ) {
  }

  private static getHeaders(headers: HttpHeaders): HttpHeaders {
    return headers || new HttpHeaders({
      'Content-Type': 'application/json'
    });
  }

  public setToken(token: ISpotifyToken): void {
    window.localStorage.setItem('spotify-auth', JSON.stringify(token));
  }

  public getToken(): ISpotifyToken {
    const token = window.localStorage.getItem('spotify-auth');
    if (token) return JSON.parse(token);
    return null;
  }

  public getJSON<T>(data: string): T {
    try {
      return JSON.parse(data);
    } catch (ex) {
      throw new Error('Invalid JSON format! Data: ' + data);
    }
  }

  public async makeRequest<T>(method: HttpMethod = 'get', url: string, data?: any, headers?: HttpHeaders, noAuth?: boolean): Promise<T> {
    headers = ApiService.getHeaders(headers);

    if (headers.get('Content-Type') === 'application/x-www-form-urlencoded') {
      const str = [];
      for (const key in data) {
        if (data.hasOwnProperty(key))
          str.push(encodeURIComponent(key) + '=' + encodeURIComponent(data[key]));
      }
      data = str.join('&');
    }

    if (!noAuth) {
      const token: ISpotifyToken = this.getToken();
      if (token) {
        headers = headers.append('Authorization', 'Bearer ' + token.access_token);
      }
    }

    switch (method) {
      case 'post':
        return this.http.post<T>(url, data, {headers}).toPromise();
      case 'patch':
        return this.http.patch<T>(url, data, {headers}).toPromise();
      case 'delete':
        return this.http.delete<T>(url, {headers}).toPromise();
      default:
        return this.http.get<T>(url, {headers}).toPromise();
    }
  }

}
