import {
  inject,
  Injectable
} from '@angular/core';

import {
  MatDialog
} from '@angular/material/dialog';

import { Observable
} from 'rxjs';

import { map } from 'rxjs/operators';
import { ConfirmDialogComponent } from '../components/confirm-dialog/confirm-dialog.component';
import { ConfirmDialogData } from '../models/confirm-dialog-data.model';


@Injectable({
  providedIn: 'root'
})
export class ConfirmDialogService {

  private readonly dialog = inject(MatDialog);

  confirm(
    data: ConfirmDialogData
  ): Observable<boolean> {

    return this.dialog
      .open(
        ConfirmDialogComponent,
        {
          width: '420px',
          disableClose: true,
          data
        }
      )
      .afterClosed()
      .pipe(
        map(result => result === true)
      );

  }

}