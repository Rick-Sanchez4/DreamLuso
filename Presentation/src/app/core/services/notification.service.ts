import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, of, throwError } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { Notification, NotificationStatus } from '../models/notification.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly apiUrl = `${environment.apiUrl}/notifications`;
  private unreadCountSubject = new BehaviorSubject<number>(0);
  public unreadCount$ = this.unreadCountSubject.asObservable();

  constructor(private http: HttpClient) {}

  getUserNotifications(userId: string): Observable<Result<Notification[]>> {
    console.log('NotificationService.getUserNotifications - Requesting notifications for userId:', userId);
    return this.http.get<Notification[]>(`${this.apiUrl}/${userId}`).pipe(
      map(notifications => {
        console.log('NotificationService.getUserNotifications - Response:', notifications);
        // Update unread count based on notifications - use proper status check
        const unreadCount = notifications.filter(n => {
          const status = n.status?.toString() || '';
          return status.toLowerCase() === NotificationStatus.Unread.toLowerCase() || 
                 status === NotificationStatus.Unread ||
                 status === 'Unread';
        }).length;
        this.unreadCountSubject.next(unreadCount);
        // Transform array response to Result format
        return { isSuccess: true, value: notifications } as Result<Notification[]>;
      }),
      catchError(error => {
        console.error('NotificationService.getUserNotifications - Error:', error);
        // Silently handle 401/404 errors and connection refused (status 0)
        if (error.status === 401 || error.status === 404 || error.status === 0) {
          this.unreadCountSubject.next(0);
          return of({ isSuccess: true, value: [] } as Result<Notification[]>);
        }
        return throwError(() => error);
      })
    );
  }

  getUnreadCount(userId: string): Observable<Result<{ unreadCount: number }>> {
    return this.http.get<{ unreadCount: number }>(`${this.apiUrl}/unread-count/${userId}`).pipe(
      map(response => {
        const unreadCount = response.unreadCount || 0;
        this.unreadCountSubject.next(unreadCount);
        return { isSuccess: true, value: { unreadCount } } as Result<{ unreadCount: number }>;
      }),
      catchError((error) => {
        console.error('NotificationService.getUnreadCount - Error:', error);
        // Silently handle 401/404 errors and connection refused (status 0)
        if (error.status === 401 || error.status === 404 || error.status === 0) {
          this.unreadCountSubject.next(0);
          return of({ isSuccess: true, value: { unreadCount: 0 } } as Result<{ unreadCount: number }>);
        }
        // Re-throw other errors
        return throwError(() => error);
      })
    );
  }

  markAsRead(notificationId: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${notificationId}/mark-read`, {});
  }

  markAllAsRead(userId: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${userId}/mark-all-read`, {});
  }
}

