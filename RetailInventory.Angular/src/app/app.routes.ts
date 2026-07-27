import { Routes } from '@angular/router';


import { authGuard } from './core/guards/auth.guard';
import { MainLayoutComponent } from './layout/main-layout-component/main-layout.component';
export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadChildren: () =>
      import('./features/auth/auth.routes')
        .then(r => r.AUTH_ROUTES)
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [

      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },

      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/auth.routes')
            .then(m => m.DASHBOARD_ROUTES)
      },
      {
        path: 'productos',
        loadChildren: () =>
          import('./features/productos/productos.routes')
            .then(m => m.PRODUCTOS_ROUTES)
      },
      {
        path: 'compras',
        loadChildren: () =>
          import('./features/compras/compras.routes')
            .then(m => m.COMPRAS_ROUTES)
      },
       {
        path: 'ventas',
        loadChildren: () =>
          import('./features/ventas/ventas.routes')
            .then(m => m.VENTAS_ROUTES)
      },
    ]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
