import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API } from '../../../core/constants/api.constants';
import { LoginRequest } from '../models/login-request.model';
import { ApiResponse } from '../../../core/Model/api-response.model';
import { ApiConfigurationService } from '../../../core/configuration/api-configuration.service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);
private readonly api = inject(ApiConfigurationService);
  login(request: LoginRequest) {
    return this.http.post<ApiResponse<string>>(
      `${this.api.baseUrl}${API.auth.login}`,
      request
    );
  }
}