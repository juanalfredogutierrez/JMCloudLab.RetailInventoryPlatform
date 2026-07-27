import { ErrorModel } from './error.model';

export interface ApiErrorResponse {
  errors: ErrorModel[];
  traceId: string;
}