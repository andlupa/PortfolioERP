import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-category-toolbar',
  standalone: true,
  templateUrl: './category-toolbar.html',
  styleUrl: './category-toolbar.scss'
})
export class CategoryToolbar {
  @Input() totalItems = 0;

  @Output() createRequested = new EventEmitter<void>();
  @Output() refreshRequested = new EventEmitter<void>();
}
