import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';

import { CommonModule } from '@angular/common';
import { ProductoService } from '../../services/producto.service';
import { Producto } from '../../models/producto.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-producto-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink
  ],
  templateUrl: './producto-list.component.html',
  styleUrl: './producto-list.component.scss'
})
export class ProductoListComponent implements OnInit {
  private readonly productoService = inject(ProductoService);
  productos = signal<Producto[]>([]);

  ngOnInit(): void {

    this.loadProductos();
  }
  private loadProductos(): void {
    this.productoService
      .getAll()
      .subscribe(response => {
        this.productos.set(
          response.data
        );
      });
  }
}