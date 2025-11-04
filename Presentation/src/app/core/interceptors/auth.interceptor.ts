import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, filter, switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<string | null> = new BehaviorSubject<string | null>(null);

  constructor(private authService: AuthService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    const authToken = this.authService.getAccessToken();

    if (authToken) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${authToken}`,
        },
      });
    }

    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          const refreshToken = this.authService.getRefreshToken();
          if (!refreshToken) {
            this.authService.logout();
            return throwError(() => error);
          }

          if (this.isRefreshing) {
            return this.refreshTokenSubject.pipe(
              filter((token: string | null) => token !== null),
              take(1),
              switchMap((newToken: string | null) => {
                const cloned = request.clone({
                  setHeaders: { Authorization: `Bearer ${newToken}` },
                });
                return next.handle(cloned);
              })
            );
          } else {
            this.isRefreshing = true;
            this.refreshTokenSubject.next(null);

            return this.authService.refreshAccessToken().pipe(
              switchMap((result: any) => {
                this.isRefreshing = false;
                if (result.isSuccess && result.value?.accessToken) {
                  const newToken = result.value.accessToken;
                  this.refreshTokenSubject.next(newToken);
                  const cloned = request.clone({
                    setHeaders: { Authorization: `Bearer ${newToken}` },
                  });
                  return next.handle(cloned);
                }
                this.authService.logout();
                return throwError(() => error);
              }),
              catchError((refreshErr: unknown) => {
                this.isRefreshing = false;
                this.authService.logout();
                return throwError(() => refreshErr);
              })
            );
          }
        }
        return throwError(() => error);
      })
    );
  }
}

