import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'sfm-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  accountActive: boolean;

  constructor() { }

  ngOnInit(): void {
  }

}
