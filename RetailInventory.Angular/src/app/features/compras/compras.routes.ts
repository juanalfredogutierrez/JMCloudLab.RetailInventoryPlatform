import { Routes } from '@angular/router';

export const COMPRAS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/compra-form.component')
        .then(m => m.CompraFormComponent)
  }
];