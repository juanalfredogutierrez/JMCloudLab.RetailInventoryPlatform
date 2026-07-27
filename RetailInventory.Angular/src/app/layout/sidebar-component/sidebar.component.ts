import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import {
  LucideAngularModule,
  LayoutDashboard,
  Package,
  ShoppingCart,
  ReceiptText,
  Boxes,
  ChevronLeft,
  ChevronRight
} from 'lucide-angular';

interface MenuItem {
  label: string;
  route: string;
  icon: any;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    LucideAngularModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Input() mobileOpen = false;

  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() closeMobileSidebar = new EventEmitter<void>();

  readonly ChevronLeft = ChevronLeft;
  readonly ChevronRight = ChevronRight;

  menuItems: MenuItem[] = [
    { label: 'Dashboard', route: '/dashboard', icon: LayoutDashboard },
    { label: 'Productos', route: '/productos', icon: Package },
    { label: 'Compras', route: '/compras', icon: ShoppingCart },
    { label: 'Ventas', route: '/ventas', icon: ReceiptText },
    // { label: 'Kardex', route: '/kardex', icon: Boxes }
  ];

  handleToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  handleCloseMobile(): void {
    this.closeMobileSidebar.emit();
  }
}