import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Notification } from '../models/notification.model';
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
    return this.http.get<Result<Notification[]>>(`${this.apiUrl}/${userId}`);
  }

  getUnreadCount(userId: string): Observable<Result<{ unreadCount: number }>> {
    return this.http.get<Result<{ unreadCount: number }>>(`${this.apiUrl}/unread-count/${userId}`).pipe(
      tap(result => {
        if (result.isSuccess && result.value) {
          this.unreadCountSubject.next(result.value.unreadCount);
        }
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

