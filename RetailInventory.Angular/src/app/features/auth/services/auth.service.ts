import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { LoginRequest } from '../models/login-request.model';
import { ApiResponse } from '../../../core/Model/api-response.model';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);

  login(request: LoginRequest) {
    return this.http.post<ApiResponse<string>>(
      `${API.gateway}${API.auth.login}`,
      request
    );
  }
}