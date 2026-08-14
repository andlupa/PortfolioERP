import {
  Component,
  inject,
  OnInit,
  signal
} from '@angular/core';

import { User } from '../../models/user';
import { UserService } from '../../services/user.service';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../auth/services/auth.service';

@Component({
  selector: 'app-user-list-page',
  standalone: true,
  imports: [DatePipe, RouterLink],
  templateUrl: './user-list-page.html',
  styleUrl: './user-list-page.scss'
})
export class UserListPage implements OnInit {
  private readonly userService =
    inject(UserService);

  readonly users = signal<User[]>([]);
  readonly loading = signal(true);
  readonly error = signal<string | null>(null);
  readonly authService = inject(AuthService);

  isCurrentUser(user: User): boolean {
    return this.authService.currentUser()?.id === user.id;
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.userService.getAll().subscribe({
      next: users => {
        this.users.set(users);
        this.loading.set(false);
      },

      error: error => {
        console.error(
          'Errore caricamento utenti',
          error
        );

        this.error.set(
          'Impossibile caricare gli utenti.'
        );

        this.loading.set(false);
      }
    });
  }

  changeStatus(user: User): void {
    this.userService
      .updateStatus(
        user.id,
        !user.isActive
      )
      .subscribe({
        next: updated => {
          this.replaceUser(updated);
        },

        error: error => {
          console.error(
            'Errore modifica stato',
            error
          );
        }
      });
  }

  changeRole(
    user: User,
    role: 'Admin' | 'User'
  ): void {
    if (user.role === role) {
      return;
    }

    this.userService
      .updateRole(user.id, role)
      .subscribe({
        next: updated => {
          this.replaceUser(updated);
        },

        error: error => {
          console.error(
            'Errore modifica ruolo',
            error
          );
        }
      });
  }

  private replaceUser(updated: User): void {
    this.users.update(users =>
      users.map(user =>
        user.id === updated.id
          ? updated
          : user
      )
    );
  }
}
