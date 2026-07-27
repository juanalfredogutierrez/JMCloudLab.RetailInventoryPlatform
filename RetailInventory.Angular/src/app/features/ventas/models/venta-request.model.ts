import { VentaDetalle } from "./venta-detalle.model";

export interface VentaRequest {
  observacion: string;
  detalles: VentaDetalle[];
}