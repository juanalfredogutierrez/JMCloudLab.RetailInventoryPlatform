import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { finalize } from 'rxjs';

import { AuthService } from '../services/auth.service';
import { TokenService } from '../../../core/services/token.service';
import { NotificationService } from '../../../core/shared/services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly notification = inject(NotificationService);
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  readonly loading = signal(false);

  readonly loginForm = this.fb.nonNullable.group({
    userName: ['admin', Validators.required],
    password: ['Admin123*', Validators.required]
  });

  login(): void {

    if (this.loginForm.invalid || this.loading()) {
      return;
    }

    this.loading.set(true);

    this.authService
      .login(this.loginForm.getRawValue())
      .pipe(
        finalize(() => this.loading.set(false))
      )
      .subscribe({
        next: response => {
          this.tokenService.setToken(response.data);
          this.notification.success(
            'Bienvenido al sistema'
          );
          this.router.navigate(['/dashboard']);
        },
        error: error => {
          this.notification.errorFromApi(
            error,
            'Error al registrar la compra'
          );
        }
      });
  }
}