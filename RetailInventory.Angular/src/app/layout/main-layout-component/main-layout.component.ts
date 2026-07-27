import { CommonModule } from '@angular/common';
import { Component, computed, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { TopbarComponent } from '../topbar-component/topbar.component';
import { SidebarComponent } from '../sidebar-component/sidebar.component';


@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    SidebarComponent,
    TopbarComponent
  ],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
  sidebarCollapsed = signal(false);
  mobileSidebarOpen = signal(false);

  layoutClasses = computed(() => ({
    'app-shell': true,
    'app-shell--sidebar-collapsed': this.sidebarCollapsed(),
    'app-shell--mobile-sidebar-open': this.mobileSidebarOpen()
  }));

  onToggleSidebar(): void {
    this.sidebarCollapsed.update(value => !value);
  }

  onToggleMobileSidebar(): void {
    this.mobileSidebarOpen.update(value => !value);
  }

  onCloseMobileSidebar(): void {
    this.mobileSidebarOpen.set(false);
  }
}