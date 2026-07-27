import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { CompraRequest } from '../models/compra-request-model';


@Injectable({
  providedIn: 'root'
})
export class CompraService {

  private readonly http = inject(HttpClient);

  create(request: CompraRequest) {

    return this.http.post<ApiResponse<string>>(
      `${API.gateway}${API.transaccion.compras}`,
      request
    );
  }
}