import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

export type ToastType = 'success' | 'danger' | 'warning' | 'info';

@Component({
  selector: 'app-toast',
  standalone: true,
  templateUrl: './app-toast.html',
  styleUrl: './app-toast.scss'
})
export class AppToast {
  @Input() visible = false;
  @Input() message = '';
  @Input() type: ToastType = 'success';

  @Output() closed = new EventEmitter<void>();

  close(): void {
    this.closed.emit();
  }
}
