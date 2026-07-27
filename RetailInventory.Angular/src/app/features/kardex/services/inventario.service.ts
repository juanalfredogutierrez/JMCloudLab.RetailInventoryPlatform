import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class InventarioService {

  private readonly http = inject(HttpClient);

  getStock(
    productoId: number
  ): Observable<ApiResponse<number>> {

    return this.http.get<ApiResponse<number>>(
      `${API.gateway}${API.inventario.stock}/${productoId}`
    );
  }
}