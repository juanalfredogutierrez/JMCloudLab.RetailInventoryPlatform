import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

import { TokenService } from '../../../core/services/token.service';
import { ConfirmDialogService } from '../../../core/shared/services/confirm-dialog.service';

@Component({
  selector: 'app-user-menu',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './user-menu.component.html',
  styleUrl: './user-menu.component.scss'
})
export class UserMenuComponent {

  private readonly tokenService = inject(TokenService);
  private readonly router = inject(Router);
  private readonly confirmDialog = inject(ConfirmDialogService);

  opened = false;

  toggle(): void {
    this.opened = !this.opened;
  }

  logout(): void {

    this.confirmDialog
      .confirm({
        title: 'Cerrar sesión',
        message: '¿Desea salir del sistema?',
        confirmText: 'Salir'
      })
      .subscribe(confirmado => {

        if (!confirmado) {
          return;
        }

        this.tokenService.clear();

        this.router.navigate([
          '/login'
        ]);

      });

  }

}