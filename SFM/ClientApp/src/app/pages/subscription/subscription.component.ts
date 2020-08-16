import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'sfm-subscription',
  templateUrl: './subscription.component.html',
  styleUrls: ['./subscription.component.scss']
})
export class SubscriptionComponent implements OnInit {

  public formGroup: FormGroup;

  constructor() {
    this.formGroup = new FormGroup({
      address: new FormGroup({
        street: new FormControl(null, [Validators.required]),
        number: new FormControl(null, [Validators.required]),
        postcode: new FormControl(null, [Validators.required]),
        city: new FormControl(null, [Validators.required])
      }),
      plan: new FormGroup({})
    });
  }

  public ngOnInit(): void {
  }

  public async proceedPayment(): Promise<void> {

  }

}
