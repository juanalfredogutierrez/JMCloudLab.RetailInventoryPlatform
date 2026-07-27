import { Injectable } from '@angular/core';

export interface CurrentUser {
  id: string;
  userName: string;
  email: string;
  role: string;
}

@Injectable({
  providedIn: 'root'
})
export class TokenService {

  private readonly TOKEN_KEY = 'token';

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  setToken(token: string): void {
    localStorage.setItem(this.TOKEN_KEY, token);
  }

  clear(): void {
    localStorage.removeItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {

    const payload = this.getPayload();

    if (!payload) {
      return false;
    }

    return payload.exp * 1000 > Date.now();
  }

  getCurrentUser(): CurrentUser | null {

    const payload = this.getPayload();

    if (!payload) {
      return null;
    }

    return {

      id: payload.nameid,

      userName: payload.unique_name ?? payload.name,

      email: payload.email,

      role: payload.role
    };
  }

  private getPayload(): any | null {

    const token = this.getToken();

    if (!token) {
      return null;
    }

    try {

      return JSON.parse(atob(token.split('.')[1]));

    } catch {

      return null;
    }
  }

  
}