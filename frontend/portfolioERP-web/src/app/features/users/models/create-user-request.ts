export interface CreateUserRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  role: 'Admin' | 'User' | 'Demo';
}
