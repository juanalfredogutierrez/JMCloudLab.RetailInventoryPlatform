import { Routes } from '@angular/router';

export const PRODUCTOS_ROUTES: Routes = [

  {
    path: '',
    loadComponent: () =>
      import('./pages/producto-list/producto-list.component')
        .then(m => m.ProductoListComponent)
  },

  {
    path: 'nuevo',
    loadComponent: () =>
      import('./pages/producto-form/producto-form.component')
        .then(m => m.ProductoFormComponent)
  }
];