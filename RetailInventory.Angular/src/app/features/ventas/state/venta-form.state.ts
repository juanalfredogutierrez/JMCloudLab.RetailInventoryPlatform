import { Injectable, inject, signal } from '@angular/core';
import { InventarioService } from '../../kardex/services/inventario.service';
import { Producto } from '../models/producto.model';
import { VentaDetalleItem } from '../models/venta-detalle-item.model';
import { IGV } from '../constants/venta.constants';

@Injectable()
export class VentaFormState {
  private readonly inventarioService = inject(InventarioService);
  readonly productos = signal<Producto[]>([]);
  readonly detalles = signal<VentaDetalleItem[]>([]);
  readonly stockDisponible = signal(0);

  cargarStock(productoId: number): void {

    this.inventarioService
      .getStock(productoId)
      .subscribe({
        next: response => {
          this.stockDisponible.set(
            response.data
          );
        },
        error: () => {
          this.stockDisponible.set(0);
        }
      });
  }

  eliminar(index: number): void {
    this.detalles.update(items =>
      items.filter((_, i) => i !== index)
    );
  }

  agregarProducto(producto: Producto, cantidad: number): string | null {
    if (cantidad <= 0) {
      return 'La cantidad debe ser mayor a cero';
    }

    if (cantidad > this.stockDisponible()) {
      return `La cantidad no debe ser mayor al stock disponible (${this.stockDisponible()})`;
    }
    
    const existe = this.detalles()
      .some(item => item.productoId === producto.productoId);

    if (existe) {
      return 'El producto ya fue agregado';
    }

    const subtotal = Number(
      (cantidad * producto.precio).toFixed(2)
    );

    const igv = Number(
      (subtotal * IGV).toFixed(2)
    );

    const total = Number(
      (subtotal + igv).toFixed(2)
    );

    const detalle: VentaDetalleItem = {
      productoId: producto.productoId,
      nombre: producto.nombre,
      precioUnitario: producto.precio,
      cantidad,
      subtotal,
      igv,
      total
    };

    this.detalles.update(items => [
      ...items,
      detalle
    ]);

    return null;
  }


}