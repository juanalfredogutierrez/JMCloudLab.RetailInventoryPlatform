import {
  Component,
  OnInit,
  inject,
  signal
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { CompraService } from '../services/compra.service';
import { ProductoService } from '../../productos/services/producto.service';
import { ProductoFormComponent } from '../../productos/pages/producto-form/producto-form.component';
import { CompraDetalleItem } from '../models/compra-detalle-item.model';
import { CompraRequest } from '../models/compra-request-model';
import { CompraFormState } from '../state/compra-form.state';
import { Producto } from '../models/producto.model';
import { NotificationService } from '../../../core/shared/services/notification.service';
import { ConfirmDialogService } from '../../../core/shared/services/confirm-dialog.service';

@Component({
  selector: 'app-compra-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DecimalPipe
  ],
  providers: [
    CompraFormState
  ],
  templateUrl: './compra-form.component.html',
  styleUrl: './compra-form.component.scss'
})
export class CompraFormComponent implements OnInit {
  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notification = inject(NotificationService);
  private readonly fb = inject(FormBuilder);
  private readonly productoService = inject(ProductoService);
  private readonly compraService = inject(CompraService);
  private readonly router = inject(Router);
  private readonly dialog = inject(MatDialog);
  private readonly compraFormState = inject(CompraFormState);
  productos = signal<Producto[]>([]);
  readonly detalles = this.compraFormState.detalles;

  readonly form = this.fb.nonNullable.group({
    productoId: [0, Validators.required],
    cantidad: [1, [Validators.required, Validators.min(1)]],
    precioUnitario: [0, [Validators.required, Validators.min(0.01)]],
    observacion: ['']
  });

  ngOnInit(): void {
    this.cargarProductos();
    this.escucharCambioProducto();
  }

  private cargarProductos(): void {
    this.productoService
      .getAll()
      .subscribe(response => {
        this.productos.set(response.data);
        if (response.data.length > 0) {
          const producto =
            response.data[
            response.data.length - 1
            ];
          this.form.patchValue({
            productoId: producto.productoId,
            precioUnitario: producto.precio
          });
        }
      });
  }

  get productoSeleccionado(): Producto | undefined {
    const productoId = this.form.getRawValue().productoId;
    return this.productos().find(
      producto => producto.productoId === productoId
    );
  }

  get subtotalActual(): number {
    const { cantidad, precioUnitario } = this.form.getRawValue();
    return Number(
      (cantidad * precioUnitario).toFixed(2)
    );
  }

  get totalGeneral(): number {
    return this.calcularTotal(
      item => item.total
    );
  }

  agregarProducto(): void {
    const producto = this.productoSeleccionado;
    if (!producto) {
      this.notification.warning('Debe seleccionar un producto');
      return;
    }

    const { cantidad, precioUnitario } = this.form.getRawValue();
    const error = this.compraFormState.agregarProducto(
      producto,
      cantidad,
      precioUnitario
    );

    if (error) {
      this.notification.warning(error);
      return;
    }

    this.form.patchValue({
      cantidad: 1
    });

  }

  eliminar(index: number): void {
    this.compraFormState.eliminar(index);
  }

  abrirModalProducto(): void {
    const dialogRef =
      this.dialog.open(
        ProductoFormComponent,
        {
          width: '700px',
          disableClose: true
        });
    dialogRef
      .afterClosed()
      .subscribe(result => {
        if (result) {
          this.cargarProductos();
        }
      });
  }

  save(): void {
    if (this.detalles().length === 0) {
      this.notification.warning(
        'Debe agregar al menos un producto'
      );
      return;
    }

    this.confirmDialog
      .confirm({
        title: 'Registrar compra',
        message: '¿Desea registrar esta compra?',
        confirmText: 'Registrar'
      })
      .subscribe(confirmado => {
        if (confirmado) {
          this.registrarCompra();
        }
      });
  }

  private registrarCompra(): void {
    const request = this.buildRequest();
    this.compraService
      .create(request)
      .subscribe({
        next: () => {
          this.notification
            .success('Compra registrada')
            .afterDismissed()
            .subscribe(() => {
              this.router.navigate([
                '/dashboard'
              ]);
            });
        },
        error: error => {
          this.notification.errorFromApi(
            error,
            'Error al registrar la compra'
          );
        }
      });
  }

  private buildRequest(): CompraRequest {
    const { observacion } = this.form.getRawValue();
    return {
      observacion,
      detalles: this.detalles().map(detalle => ({
        productoId: detalle.productoId,
        cantidad: detalle.cantidad,
        precioUnitario: detalle.precioUnitario
      }))
    };
  }

  private calcularTotal(
    selector: (item: CompraDetalleItem) => number
  ): number {

    return Number(
      this.detalles()
        .reduce(
          (total, item) => total + selector(item),
          0
        ).toFixed(2)
    );
  }

  private escucharCambioProducto(): void {
    this.form.controls.productoId
      .valueChanges
      .subscribe(productoId => {
        const producto = this.productos()
          .find(item => item.productoId === productoId);
        if (!producto) {
          return;
        }
        this.form.patchValue({
          precioUnitario: producto.precio
        });
      });
  }
} 