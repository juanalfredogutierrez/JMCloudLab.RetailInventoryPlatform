import {
  Component,
  inject,
  OnInit
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { DecimalPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ProductoService } from '../../productos/services/producto.service';
import { VentaService } from '../services/venta.service';
import { VentaFormState } from '../state/venta-form.state';
import { Producto } from '../models/producto.model';

import { VentaDetalleItem } from '../models/venta-detalle-item.model';
import { IGV } from '../constants/venta.constants';
import { VentaRequest } from '../models/venta-request.model';
import { NotificationService } from '../../../core/shared/services/notification.service';
import { ConfirmDialogService } from '../../../core/shared/services/confirm-dialog.service';

@Component({
  selector: 'app-venta-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink,
    DecimalPipe
  ],
  providers: [
    VentaFormState
  ],
  templateUrl: './venta-form.component.html',
  styleUrl: './venta-form.component.scss'
})
export class VentaFormComponent implements OnInit {

  private readonly confirmDialog = inject(ConfirmDialogService);
  private readonly notification = inject(NotificationService);
  private readonly fb = inject(FormBuilder);
  private readonly productoService = inject(ProductoService);
  private readonly ventaService = inject(VentaService);
  private readonly router = inject(Router);
  private readonly ventaFormState = inject(VentaFormState);
  readonly productos = this.ventaFormState.productos;
  readonly detalles = this.ventaFormState.detalles;
  readonly stockDisponible = this.ventaFormState.stockDisponible;

  readonly form = this.fb.nonNullable.group({
    productoId: [0, Validators.required],
    cantidad: [1,
      [
        Validators.required,
        Validators.min(1)
      ]
    ],

    observacion: ['']

  });

  ngOnInit(): void {
    this.cargarProductos();
    this.escucharCambioProducto();
  }

  get productoSeleccionado(): Producto | undefined {
    const productoId = this.form.getRawValue().productoId;
    return this.productos().find(
      producto => producto.productoId === productoId
    );
  }

  get precioSeleccionado(): number {
    return this.productoSeleccionado?.precio ?? 0;
  }

  get subtotalActual(): number {
    return Number(
      (
        this.form.getRawValue().cantidad *
        this.precioSeleccionado
      ).toFixed(2)
    );

  }

  get igvActual(): number {
    return Number(
      (
        this.subtotalActual * IGV
      ).toFixed(2)
    );
  }

  get totalActual(): number {
    return Number(
      (
        this.subtotalActual +
        this.igvActual
      ).toFixed(2)
    );
  }

  get subtotalGeneral(): number {
    return this.calcularTotal(
      item => item.subtotal
    );
  }

  get igvGeneral(): number {
    return this.calcularTotal(
      item => item.igv
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

    const cantidad = this.form.getRawValue().cantidad;
    const error = this.ventaFormState.agregarProducto(
      producto,
      cantidad
    );

    if (error) {
      this.notification.warning(error);
      return;
    }

    this.form.patchValue({ cantidad: 1 });

  }

  eliminar(index: number): void {
    this.ventaFormState.eliminar(index);
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
        title: 'Registrar venta',
        message: '¿Desea registrar esta venta?',
        confirmText: 'Registrar'
      })
      .subscribe(confirmado => {

        if (!confirmado) {
          return;
        }

        this.registrarVenta();

      });

  }

  private registrarVenta(): void {
    const request = this.buildRequest();
    this.ventaService
      .create(request)
      .subscribe({
        next: () => {
          this.notification
            .success('Venta registrada')
            .afterDismissed()
            .subscribe(() => {

              this.router.navigate([
                '/dashboard'
              ]);

            });

        },

        error: error => {
          if (error?.error?.errors?.length > 0) {

            const errorMessages = error.error.errors
              .map((x: { message: string }) => x.message)
              .join('\n');

            this.notification.error(
              errorMessages
            );
            return;
          }

          this.notification.errorFromApi(
            error,
            'Error al registrar la venta'
          );

        }

      });

  }

  private calcularTotal(selector: (item: VentaDetalleItem) => number): number {
    return Number(
      this.detalles()
        .reduce(
          (total, item) => total + selector(item),
          0
        )
        .toFixed(2)
    );
  }

  private cargarProductos(): void {

    this.productoService
      .getAll()
      .subscribe(response => {
        this.productos.set(response.data);
        if (!response.data.length) {
          return;
        }

        const productoId = response.data[0].productoId;
        this.form.patchValue({
          productoId
        });
        this.ventaFormState.cargarStock(productoId);

      });

  }
  private escucharCambioProducto(): void {
    this.form.controls.productoId
      .valueChanges
      .subscribe(productoId => {
        if (productoId > 0) {
          this.ventaFormState.cargarStock(productoId);
        }
      });
  }

  private buildRequest(): VentaRequest {
    const { observacion } =
      this.form.getRawValue();
    return {
      observacion,
      detalles: this.detalles().map(detalle => ({
        productoId: detalle.productoId,
        cantidad: detalle.cantidad,
        precioUnitario: detalle.precioUnitario

      }))
    };

  }
}