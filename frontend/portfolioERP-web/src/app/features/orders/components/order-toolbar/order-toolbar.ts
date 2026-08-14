import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-order-toolbar',
  standalone: true,
  templateUrl: './order-toolbar.html',
  styleUrl: './order-toolbar.scss'
})
export class OrderToolbar {
  @Input() totalItems = 0;

  @Output() createRequested = new EventEmitter<void>();
  @Output() refreshRequested = new EventEmitter<void>();
}
