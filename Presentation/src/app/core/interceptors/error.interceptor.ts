import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ToastService } from '../services/toast.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private toastService: ToastService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ocorreu um erro inesperado';

        if (error.error?.message) {
          errorMessage = error.error.message;
        } else if (error.error?.errors) {
          // FluentValidation errors
          const validationErrors = error.error.errors;
          const firstError = Object.values(validationErrors)[0] as string[];
          errorMessage = firstError[0];
        } else if (error.message) {
          errorMessage = error.message;
        }

        // Não mostrar toast para erros 401 (handled by auth interceptor)
        if (error.status !== 401) {
          this.toastService.error(errorMessage);
        }

        return throwError(() => error);
      })
    );
  }
}

