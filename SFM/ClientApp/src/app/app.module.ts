import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatRippleModule } from '@angular/material/core';
import { HttpClientModule } from "@angular/common/http";
import { MatButtonModule } from "@angular/material/button";

@NgModule({
  declarations: [
    AppComponent
  ],
    imports: [
        BrowserModule,
        HttpClientModule,
        AppRoutingModule,
        BrowserAnimationsModule,
        MatSidenavModule,
        MatToolbarModule,
        MatListModule,
        MatIconModule,
        MatRippleModule,
        MatButtonModule,
    ],
  providers: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule {

}
