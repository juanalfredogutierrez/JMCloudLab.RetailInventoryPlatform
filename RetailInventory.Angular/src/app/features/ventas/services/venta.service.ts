import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { VentaRequest } from '../models/venta-request.model';




@Injectable({
  providedIn: 'root'
})
export class VentaService {

  private readonly http = inject(HttpClient);

  create(request: VentaRequest) {

    return this.http.post<ApiResponse<string>>(
      `${API.gateway}${API.transaccion.ventas}`,
      request
    );
  }
}