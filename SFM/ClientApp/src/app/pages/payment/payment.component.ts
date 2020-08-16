import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'sfm-payment',
  templateUrl: './payment.component.html',
  styleUrls: ['./payment.component.scss']
})
export class PaymentComponent implements OnInit {

  constructor(
    private route: ActivatedRoute
  ) {
    this.route.params.subscribe(params => {
      if (params.success) {
      } else {
      }
      window.close();
    });
  }

  public ngOnInit(): void {
  }

}
