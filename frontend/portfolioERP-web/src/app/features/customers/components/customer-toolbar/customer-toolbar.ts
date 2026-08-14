import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-customer-toolbar',
  standalone: true,
  templateUrl: './customer-toolbar.html',
  styleUrl: './customer-toolbar.scss'
})
export class CustomerToolbar {
  @Input() totalItems = 0;

  @Output() createRequested = new EventEmitter<void>();
  @Output() refreshRequested = new EventEmitter<void>();
}
