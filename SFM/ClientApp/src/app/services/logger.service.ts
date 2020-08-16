import { Injectable } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoggerComponent } from '../components/logger/logger.component';

export enum LogType {
  Error,
  Success,
  Warning,
  Info,
}

@Injectable({
  providedIn: 'root'
})
export class LoggerService {

  constructor(
    private snackBar: MatSnackBar,
  ) {
  }

  public log(type: LogType, message: any): void {
    console.log(message);
    this.snackBar.openFromComponent(LoggerComponent, {
      duration: 1500,
      horizontalPosition: 'left',
      data: {
        message: this.getMessage(message),
        type,
      },
    });
  }

  private getMessage(err: any): string {
    let msg;
    if (typeof err === 'string') msg = err;
    else if (err.msg) msg = err.msg;
    else if (err.error?.msg) msg = err.error.msg;
    else if (err.error?.message) msg = err.error.message;
    else if (err.error?.errors && err.error?.errors[0]?.message) msg = err.error.errors[0].message;
    else if (err.message) msg = err.message;
    else if (err.Message) msg = err.Message;
    return msg;
  }

}
