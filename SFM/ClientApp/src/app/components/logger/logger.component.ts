import { Component, Inject, OnInit } from '@angular/core';
import { MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { LogType } from '../../services/logger.service';

@Component({
  selector: 'sfm-logger',
  templateUrl: './logger.component.html',
  styleUrls: ['./logger.component.scss']
})
export class LoggerComponent implements OnInit {

  public message: string;
  public type: LogType;
  public types = LogType;

  constructor(
    @Inject(MAT_SNACK_BAR_DATA) private data: { message: string, type: LogType }
  ) {
    this.message = this.data.message;
    this.type = this.data.type;
  }

  public ngOnInit(): void {
  }

}
