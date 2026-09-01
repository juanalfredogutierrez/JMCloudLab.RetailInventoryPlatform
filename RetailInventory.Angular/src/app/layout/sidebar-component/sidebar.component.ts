import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

import {
  LucideChevronLeft,
  LucideChevronRight,
  LucideDynamicIcon,
  LucideLayoutDashboard,
  LucidePackage,
  LucideReceiptText,
  LucideShoppingCart
} from '@lucide/angular';

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
    LucideDynamicIcon
  ],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss'
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Input() mobileOpen = false;

  @Output() toggleSidebar = new EventEmitter<void>();
  @Output() closeMobileSidebar = new EventEmitter<void>();

  readonly ChevronLeft = LucideChevronLeft;
  readonly ChevronRight = LucideChevronRight;

  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      route: '/dashboard',
      icon: LucideLayoutDashboard
    },
    {
      label: 'Productos',
      route: '/productos',
      icon: LucidePackage
    },
    {
      label: 'Compras',
      route: '/compras',
      icon: LucideShoppingCart
    },
    {
      label: 'Ventas',
      route: '/ventas',
      icon: LucideReceiptText
    }
    // { label: 'Kardex', route: '/kardex', icon: LucideBoxes }
  ];

  handleToggleSidebar(): void {
    this.toggleSidebar.emit();
  }

  handleCloseMobile(): void {
    this.closeMobileSidebar.emit();
  }
}