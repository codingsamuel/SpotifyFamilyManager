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

export interface ISpotifyExternalUrls {
  spotify: string;
}

export interface ISpotifyImage {
  height?: any;
  url: string;
  width?: any;
}