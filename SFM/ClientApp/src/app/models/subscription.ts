export interface ISubscription {
  id: number;
  spotifyUserId: number;
  paymentInterval: number;
  price: number;
  lastPayment: Date;
  token: string;
  active: boolean;
}