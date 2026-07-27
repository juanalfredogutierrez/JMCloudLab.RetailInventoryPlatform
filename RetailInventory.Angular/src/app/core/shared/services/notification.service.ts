import { HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  MatSnackBar,
  MatSnackBarRef,
  TextOnlySnackBar
} from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  private readonly snackBar = inject(MatSnackBar);

  private open(
    message: string,
    panelClass: string
  ): MatSnackBarRef<TextOnlySnackBar> {

    return this.snackBar.open(
      message,
      'Cerrar',
      {
        duration: 3500,
        horizontalPosition: 'right',
        verticalPosition: 'top',
        panelClass: [panelClass]
      }
    );

  }

  success(message: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.open(message, 'success-snackbar');
  }

  error(message: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.open(message, 'error-snackbar');
  }

  warning(message: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.open(message, 'warning-snackbar');
  }

  info(message: string): MatSnackBarRef<TextOnlySnackBar> {
    return this.open(message, 'info-snackbar');
  }

  errorFromApi(error: HttpErrorResponse, defaultMessage = 'Ocurrió un error'): void {
    const message =
      error.error?.errors?.[0]?.message ??
      defaultMessage;
    this.error(message);
  }
}