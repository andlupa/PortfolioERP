import { HttpErrorResponse } from '@angular/common/http';
import {
  Component,
  inject,
  signal
} from '@angular/core';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-user-form-page',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './user-form-page.html',
  styleUrl: './user-form-page.scss'
})
export class UserFormPage {
  private readonly formBuilder = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  readonly saving = signal(false);
  readonly error = signal<string | null>(null);

  readonly form = this.formBuilder.nonNullable.group({
    username: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],

    email: [
      '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(200)
      ]
    ],

    firstName: [
      '',
      Validators.required
    ],

    lastName: [
      '',
      Validators.required
    ],

    password: [
      '',
      [
        Validators.required,
        Validators.minLength(8)
      ]
    ],

    role: this.formBuilder.nonNullable.control <
      'Admin' | 'User' | 'Demo'
    >('User')
  });

  save(): void {
    this.error.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    this.saving.set(true);

    this.userService
      .create(this.form.getRawValue())
      .subscribe({
        next: () => {
          void this.router.navigate(['/users']);
        },

        error: (error: HttpErrorResponse) => {
          console.error(
            'Errore creazione utente',
            error
          );

          const message = error.error?.message;

          if (message === 'Username already exists.') {
            this.error.set(
              'Lo username inserito è già utilizzato.'
            );
          } else if (
            message === 'Email already exists.'
          ) {
            this.error.set(
              'L’indirizzo email inserito è già utilizzato.'
            );
          } else if (error.status === 400) {
            this.error.set(
              'Controlla i dati inseriti.'
            );
          } else {
            this.error.set(
              'Impossibile creare l’utente.'
            );
          }

          this.saving.set(false);
        }

      });
  }
}
