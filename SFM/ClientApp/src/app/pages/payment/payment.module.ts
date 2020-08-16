import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PaymentComponent } from './payment.component';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  {
    path: ':response',
    component: PaymentComponent
  }
];

@NgModule({
  declarations: [PaymentComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
  ]
})
export class PaymentModule {
}
