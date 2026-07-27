import { Component, inject } from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { Router } from '@angular/router';
import { MatDialogRef } from '@angular/material/dialog';
import { ProductoService } from '../../services/producto.service';
import { NotificationService } from '../../../../core/shared/services/notification.service';
import { ConfirmDialogService } from '../../../../core/shared/services/confirm-dialog.service';

@Component({
  selector: 'app-producto-form',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './producto-form.component.html',
  styleUrl: './producto-form.component.scss'
})
export class ProductoFormComponent {
  private readonly fb = inject(FormBuilder);
  private readonly productoService = inject(ProductoService);
  private readonly notification = inject(NotificationService);
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly router = inject(Router);
  private readonly dialogRef = inject(
    MatDialogRef<ProductoFormComponent>,
    {
      optional: true
    });

  readonly form = this.fb.nonNullable.group({
    codigo: ['', Validators.required],
    nombre: ['', Validators.required],
    descripcion: [''],
    precio: [0, [Validators.required, Validators.min(0.01)]]
  });

  save(): void {

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.notification.warning(
        'Complete los campos obligatorios.'
      );
      return;
    }

    this.confirmDialog
      .confirm({
        title: 'Registrar producto',
        message: '¿Desea registrar este producto?',
        confirmText: 'Registrar'
      })
      .subscribe(confirmado => {
        if (!confirmado) {
          return;
        }
        this.registrarProducto();
      });

  }

  cancelar(): void {
    if (this.dialogRef) {
      this.dialogRef.close();
      return;

    }

    this.router.navigate([
      '/productos'
    ]);

  }

  private registrarProducto(): void {
    this.productoService
      .create(this.form.getRawValue())
      .subscribe({
        next: () => {
          this.notification.success(
            'Producto registrado correctamente.'
          );

          if (this.dialogRef) {
            this.dialogRef.close(true);
            return;
          }

          this.router.navigate([
            '/productos'
          ]);
        },
        error: error => {
          const mensaje =
            error?.error?.errors?.[0]?.message ??
            'Ocurrió un error al registrar el producto.';
          this.notification.errorFromApi(mensaje);
        }
      });
  }
}