export interface VentaDetalleItem {
  productoId: number;
  nombre: string;
  precioUnitario: number;
  cantidad: number;
  subtotal: number;
  igv: number;
  total: number;
}