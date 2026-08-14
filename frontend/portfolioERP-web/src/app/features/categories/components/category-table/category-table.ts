import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';
import { CommonModule } from '@angular/common';

import { Category } from '../../models/category';

@Component({
  selector: 'app-category-table',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-table.html',
  styleUrl: './category-table.scss'
})
export class CategoryTable {
  @Input() categories: Category[] = [];
  @Input() deactivatingCategoryId: number | null = null;

  @Output() editRequested = new EventEmitter<number>();
  @Output() deactivateRequested =
    new EventEmitter<Category>();

  edit(id: number): void {
    this.editRequested.emit(id);
  }

  deactivate(category: Category): void {
    this.deactivateRequested.emit(category);
  }
}
