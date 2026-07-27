import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, computed, inject, signal } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { filter } from 'rxjs/operators';

import {
  Bell,
  ChevronRight,
  Menu,
  Mail,
  LucideAngularModule,
  PanelLeftClose,
  PanelLeftOpen
} from 'lucide-angular';
import { TokenService } from '../../core/services/token.service';
import { UserMenuComponent } from '../components/user-menu/user-menu.component';

interface BreadcrumbItem {
  label: string;
  active?: boolean;
}

@Component({
  selector: 'app-topbar-component',
  standalone: true,
  imports: [CommonModule, LucideAngularModule,UserMenuComponent],
  templateUrl: './topbar.component.html',
  styleUrl: './topbar.component.scss'
})
export class TopbarComponent {
  private readonly router = inject(Router);
  private readonly tokenService = inject(TokenService);
  @Input() sidebarCollapsed = false;

  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() toggleMobileSidebar = new EventEmitter<void>();

  readonly Bell = Bell;
  readonly Mail = Mail;
  readonly Menu = Menu;
  readonly ChevronRight = ChevronRight;
  readonly PanelLeftClose = PanelLeftClose;
  readonly PanelLeftOpen = PanelLeftOpen;

  private readonly currentUrl = signal(this.router.url);

  readonly currentUser = signal(
    this.tokenService.getCurrentUser()
  );
  readonly breadcrumb = computed(() => this.buildBreadcrumb(this.currentUrl()));

  constructor() {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.currentUrl.set(event.urlAfterRedirects ?? event.url);
      });
  }

  private buildBreadcrumb(url: string): BreadcrumbItem[] {
    const cleanUrl = url.split('?')[0].split('#')[0];

    if (cleanUrl.startsWith('/dashboard')) {
      return [{ label: 'Dashboard', active: true }];
    }

    if (cleanUrl.startsWith('/productos')) {
      return [{ label: 'Productos' }, { label: 'Catálogo', active: true }];
    }

    if (cleanUrl.startsWith('/compras')) {
      return [{ label: 'Compras' }, { label: 'Registrar compra', active: true }];
    }

    if (cleanUrl.startsWith('/ventas')) {
      return [{ label: 'Ventas' }, { label: 'Registrar venta', active: true }];
    }

    if (cleanUrl.startsWith('/kardex')) {
      return [{ label: 'Kardex' }, { label: 'Movimientos', active: true }];
    }

    return [{ label: 'Retail Inventory', active: true }];
  }

  handleDesktopToggle(): void {
    this.toggleSidebar.emit();
  }

  handleMobileToggle(): void {
    this.toggleMobileSidebar.emit();
  }
  get avatar(): string {

    const user = this.currentUser();

    if (!user?.userName)
      return '?';

    return user.userName
      .split(' ')
      .map(x => x[0])
      .join('')
      .substring(0, 2)
      .toUpperCase();

  }
  get displayRole(): string {

    switch (this.currentUser()?.role) {

      case 'ADMIN':
        return 'Administrador';

      default:
        return this.currentUser()?.role ?? '';

    }

  }
}