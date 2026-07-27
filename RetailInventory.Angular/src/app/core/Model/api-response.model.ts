export interface ApiResponse<T> {
  data: T;
  traceId: string;
}