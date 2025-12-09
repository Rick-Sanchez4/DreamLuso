import { Environment } from './environment.interface';

export const environment: Environment = {
  production: false,
  apiUrl: 'http://localhost:5149/api',
  baseUrl: 'http://localhost:5149' // Base URL without /api for static files
};
