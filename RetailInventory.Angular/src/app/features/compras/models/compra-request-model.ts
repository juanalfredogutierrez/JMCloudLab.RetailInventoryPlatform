import { CompraDetalle } from './compra-detalle.model';

export interface CompraRequest {
  observacion: string;
  detalles: CompraDetalle[];
}