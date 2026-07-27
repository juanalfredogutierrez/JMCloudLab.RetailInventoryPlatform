import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { CompraRequest } from '../models/compra-request-model';
import { ApiConfigurationService } from '../../../core/configuration/api-configuration.service';

@Injectable({
  providedIn: 'root',
})
export class CompraService {
  private readonly http = inject(HttpClient);
  private readonly api = inject(ApiConfigurationService);
  create(request: CompraRequest) {
    return this.http.post<ApiResponse<string>>(
      `${this.api.baseUrl}${API.transaccion.compras}`,
      request,
    );
  }
}
