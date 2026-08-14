import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-product-toolbar',
  standalone: true,
  templateUrl: './product-toolbar.html',
  styleUrl: './product-toolbar.scss'
})
export class ProductToolbar {
  @Input() totalItems = 0;

  @Output() createRequested = new EventEmitter<void>();
  @Output() refreshRequested = new EventEmitter<void>();
}
