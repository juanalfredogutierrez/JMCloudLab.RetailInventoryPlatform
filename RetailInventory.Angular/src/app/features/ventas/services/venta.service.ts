import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { VentaRequest } from '../models/venta-request.model';
import { ApiConfigurationService } from '../../../core/configuration/api-configuration.service';




@Injectable({
  providedIn: 'root'
})
export class VentaService {

  private readonly http = inject(HttpClient);
private readonly api = inject(ApiConfigurationService);
  create(request: VentaRequest) {

    return this.http.post<ApiResponse<string>>(
      `${this.api.baseUrl}${API.transaccion.ventas}`,
      request
    );
  }
}