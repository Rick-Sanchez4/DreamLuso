export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phone?: string;
  role: UserRole;
  profileImageUrl?: string;
  isActive: boolean;
}

export enum UserRole {
  Admin = 'Admin',
  RealEstateAgent = 'RealEstateAgent',
  Client = 'Client'
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: User;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  phone?: string;
  role: UserRole;
}

