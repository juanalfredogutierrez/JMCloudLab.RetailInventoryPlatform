import { Injectable, signal } from '@angular/core';
import { CompraDetalleItem } from '../models/compra-detalle-item.model';
import { Producto } from '../models/producto.model';

@Injectable()
export class CompraFormState {
  readonly detalles = signal<CompraDetalleItem[]>([]);

  agregarProducto(
    producto: Producto,
    cantidad: number,
    precioUnitario: number
  ): string | null {

    if (cantidad <= 0) {
      return 'La cantidad debe ser mayor a cero';
    }

    const existe = this.detalles()
                    .some(item => item.productoId === producto.productoId);

    if (existe) {
      return 'El producto ya fue agregado';
    }

    const total = Number(
      (cantidad * precioUnitario).toFixed(2)
    );

    const detalle: CompraDetalleItem = {
      productoId: producto.productoId,
      nombre: producto.nombre,
      cantidad,
      precioUnitario,
      total
    };

    this.detalles.update(items => [
      ...items,
      detalle
    ]);
    return null;
  }

  eliminar(index: number): void {
    this.detalles.update(items =>
      items.filter((_, i) => i !== index)
    );
  }

}