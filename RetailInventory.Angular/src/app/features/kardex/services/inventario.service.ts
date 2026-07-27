import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { ApiConfigurationService } from '../../../core/configuration/api-configuration.service';

@Injectable({
  providedIn: 'root'
})
export class InventarioService {

  private readonly http = inject(HttpClient);
private readonly api = inject(ApiConfigurationService);
  getStock(
    productoId: number
  ): Observable<ApiResponse<number>> {

    return this.http.get<ApiResponse<number>>(
      `${this.api.baseUrl}${API.inventario.stock}/${productoId}`
    );
  }
}