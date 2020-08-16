import { Injectable } from '@angular/core';
import { LoaderComponent } from '../components/loader/loader.component';

@Injectable({
  providedIn: 'root'
})
export class LoaderService {

  private loaderVisible: boolean;
  private loader: LoaderComponent;

  constructor() {
  }

  public get visible(): boolean {
    return this.loaderVisible;
  }

  public setLoader(loader: LoaderComponent): void {
    this.loader = loader;
  }

  public show(): void {
    this.loaderVisible = true;
  }

  public hide(): void {
    this.loaderVisible = false;
  }
}
