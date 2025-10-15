import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { LoginRequest, LoginResponse, RegisterRequest, User } from '../models/user.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  public redirectUrl: string | null = null;

  constructor(
    private http: HttpClient,
    private router: Router
  ) {
    this.loadUserFromStorage();
  }

  login(credentials: LoginRequest): Observable<Result<LoginResponse>> {
    return this.http.post<any>(`${this.apiUrl}/accounts/login`, credentials).pipe(
      tap(response => {
        if (response) {
          this.setTokens(response.token || response.accessToken, response.refreshToken);
          const user: User = {
            id: response.userId || response.user?.id,
            email: response.email || response.user?.email,
            firstName: response.firstName || response.user?.firstName,
            lastName: response.lastName || response.user?.lastName,
            role: response.role || response.user?.role,
            isActive: true,
            profileImageUrl: response.profileImageUrl || response.user?.profileImageUrl
          };
          this.setCurrentUser(user);
        }
      }),
      map(response => ({ isSuccess: true, value: response } as Result<LoginResponse>)),
      catchError(error => of({ isSuccess: false, error: error.error } as Result<LoginResponse>))
    );
  }

  register(userData: RegisterRequest): Observable<Result<void>> {
    return this.http.post<void>(`${this.apiUrl}/accounts/register`, userData).pipe(
      map(() => ({ isSuccess: true } as Result<void>)),
      catchError(error => of({ isSuccess: false, error: error.error } as Result<void>))
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('current_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  refreshAccessToken(): Observable<Result<LoginResponse>> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      return of({ isSuccess: false, error: { code: 'NO_REFRESH_TOKEN', description: 'No refresh token available' } } as Result<LoginResponse>);
    }

    return this.http.post<any>(`${this.apiUrl}/accounts/refresh-token`, { refreshToken }).pipe(
      tap(response => {
        if (response?.token) {
          this.setTokens(response.token, response.refreshToken);
        }
      }),
      map(response => ({ isSuccess: true, value: response } as Result<LoginResponse>)),
      catchError(error => of({ isSuccess: false, error: error.error } as Result<LoginResponse>))
    );
  }

  isAuthenticated(): boolean {
    return !!this.getAccessToken();
  }

  getAccessToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  private setTokens(accessToken: string, refreshToken: string): void {
    localStorage.setItem('access_token', accessToken);
    localStorage.setItem('refresh_token', refreshToken);
  }

  private setCurrentUser(user: User): void {
    localStorage.setItem('current_user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private loadUserFromStorage(): void {
    const userJson = localStorage.getItem('current_user');
    if (userJson) {
      try {
        const user = JSON.parse(userJson) as User;
        this.currentUserSubject.next(user);
      } catch {
        localStorage.removeItem('current_user');
      }
    }
  }
}

