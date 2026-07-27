import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { Producto } from '../models/producto.model';
import { CreateProductoRequest } from '../models/create-producto.model';
import { ApiConfigurationService } from '../../../core/configuration/api-configuration.service';

@Injectable({
  providedIn: 'root'
})
export class ProductoService {

  private readonly http = inject(HttpClient);   
  private readonly api = inject(ApiConfigurationService);
  getAll() {

    return this.http.get<ApiResponse<Producto[]>>(
      `${this.api.baseUrl}${API.productos.list}`
    );
  }

  create(request: CreateProductoRequest) {

    return this.http.post<ApiResponse<string>>(
      `${this.api.baseUrl}${API.productos.create}`,
      request
    );
  }
}