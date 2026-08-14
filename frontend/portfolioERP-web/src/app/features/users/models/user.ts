export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'User';
  isActive: boolean;
  createdAt: string;
}
