import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { Producto } from '../models/producto.model';
import { CreateProductoRequest } from '../models/create-producto.model';

@Injectable({
  providedIn: 'root'
})
export class ProductoService {

  private readonly http = inject(HttpClient);

  getAll() {

    return this.http.get<ApiResponse<Producto[]>>(
      `${API.gateway}${API.productos.list}`
    );
  }

  create(request: CreateProductoRequest) {

    return this.http.post<ApiResponse<string>>(
      `${API.gateway}${API.productos.create}`,
      request
    );
  }
}