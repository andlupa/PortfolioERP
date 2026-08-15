export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'User' | 'Demo';
  isActive: boolean;
  createdAt: string;
}
