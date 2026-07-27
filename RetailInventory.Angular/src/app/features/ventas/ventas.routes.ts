import { Routes } from '@angular/router';

export const VENTAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/venta-form.component')
        .then(m => m.VentaFormComponent)
  }
];