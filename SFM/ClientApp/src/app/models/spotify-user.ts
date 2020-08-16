export interface ISpotifyUser {
  display_name: string;
  email: string;
  external_urls: ISpotifyExternalUrls;
  href: string;
  id: string;
  images: ISpotifyImage[];
  product: string;
  type: string;
  uri: string;
}

export class SpotifyUser {
  public id: number;
  public displayName: string;
  public email: string;
  public apiUrl: string;
  public spotifyId: string;
  public imageUrl: string;
  public product: string;
  public type: string;
  public uri: string;
  public created: Date;
  public update: Date;

  constructor(user: ISpotifyUser) {
    this.displayName = user.display_name;
    this.email = user.email;
    this.apiUrl = user.href;
    this.spotifyId = user.id;
    this.imageUrl = user.images[0].url;
    this.product = user.product;
    this.type = user.type;
    this.uri = user.uri;
  }
}

export interface ISpotifyExternalUrls {
  spotify: string;
}

export interface ISpotifyImage {
  height?: any;
  url: string;
  width?: any;
}