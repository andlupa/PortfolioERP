import {
  Component,
  EventEmitter,
  Input,
  Output
} from '@angular/core';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  templateUrl: './confirm-dialog.html',
  styleUrl: './confirm-dialog.scss'
})
export class ConfirmDialog {
  @Input() open = false;
  @Input() title = 'Conferma';
  @Input() message = '';
  @Input() confirmText = 'Conferma';
  @Input() cancelText = 'Annulla';
  @Input() busy = false;
  @Input() danger = false;

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  confirm(): void {
    if (!this.busy) {
      this.confirmed.emit();
    }
  }

  cancel(): void {
    if (!this.busy) {
      this.cancelled.emit();
    }
  }
}
