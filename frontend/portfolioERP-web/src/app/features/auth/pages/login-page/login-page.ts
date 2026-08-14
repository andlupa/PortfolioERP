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
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.scss'
})
export class LoginPage {
  private readonly formBuilder =
    inject(FormBuilder);

  private readonly authService =
    inject(AuthService);

  private readonly router =
    inject(Router);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  readonly form =
    this.formBuilder.nonNullable.group({
      username: [
        '',
        Validators.required
      ],
      password: [
        '',
        Validators.required
      ]
    });

  login(): void {
    this.error.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    this.loading.set(true);

    this.authService
      .login(this.form.getRawValue())
      .subscribe({
        next: () => {
          void this.router.navigate([
            '/dashboard'
          ]);
        },

        error: (
          error: HttpErrorResponse
        ) => {
          console.error(
            'Errore login',
            error
          );

          if (error.status === 401) {
            this.error.set(
              'Username o password non validi.'
            );
          } else {
            this.error.set(
              'Impossibile effettuare l’accesso.'
            );
          }

          this.loading.set(false);
        }
      });
  }
}
