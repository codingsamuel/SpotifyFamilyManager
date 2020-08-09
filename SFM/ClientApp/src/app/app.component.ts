import { Component } from '@angular/core';
import { NavGroup } from './models/nav-item';

@Component({
  selector: 'sfm-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  navGroup: NavGroup = {
    name: 'General',
    items: [
      {
        name: 'Sprache',
        action: () => {
          return;
        }
      }
    ]
  };

}