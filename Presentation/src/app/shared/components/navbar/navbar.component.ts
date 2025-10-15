import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { User, UserRole } from '../../../core/models/user.model';
import { Notification } from '../../../core/models/notification.model';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit {
  currentUser: User | null = null;
  notifications: Notification[] = [];
  unreadCount: number = 0;
  showNotifications: boolean = false;

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
      if (user) {
        this.loadNotifications();
      }
    });

    this.notificationService.unreadCount$.subscribe(count => {
      this.unreadCount = count;
    });
  }

  loadNotifications(): void {
    if (this.currentUser) {
      this.notificationService.getUserNotifications(this.currentUser.id).subscribe(result => {
        if (result.isSuccess && result.value) {
          this.notifications = result.value;
        }
      });

      this.notificationService.getUnreadCount(this.currentUser.id).subscribe();
    }
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
  }

  handleNotificationClick(notification: Notification): void {
    this.notificationService.markAsRead(notification.id).subscribe(() => {
      this.loadNotifications();
    });

    // Navigate based on notification type
    if (notification.referenceId) {
      switch (notification.type) {
        case 'Proposal':
          this.router.navigate(['/client/proposals', notification.referenceId]);
          break;
        case 'Visit':
          this.router.navigate(['/client/visits']);
          break;
        case 'PropertyUpdate':
          this.router.navigate(['/property', notification.referenceId]);
          break;
      }
    }

    this.showNotifications = false;
  }

  markAllAsRead(): void {
    if (this.currentUser) {
      this.notificationService.markAllAsRead(this.currentUser.id).subscribe(() => {
        this.loadNotifications();
      });
    }
  }

  navigateTo(path: string): void {
    this.router.navigate([path]);
  }

  navigateToDashboard(): void {
    if (this.currentUser) {
      switch (this.currentUser.role) {
        case UserRole.Admin:
          this.router.navigate(['/admin/dashboard']);
          break;
        case UserRole.RealEstateAgent:
          this.router.navigate(['/agent/dashboard']);
          break;
        case UserRole.Client:
          this.router.navigate(['/client/dashboard']);
          break;
      }
    }
  }

  logout(): void {
    this.authService.logout();
  }

  getNotificationClass(notification: Notification): string {
    return notification.status === 'Unread' 
      ? 'bg-primary-50 border-l-4 border-primary-500' 
      : 'bg-white';
  }

  getRelativeTime(date: Date): string {
    const now = new Date();
    const diffMs = now.getTime() - new Date(date).getTime();
    const diffMins = Math.floor(diffMs / 60000);
    
    if (diffMins < 1) return 'agora mesmo';
    if (diffMins < 60) return `há ${diffMins} min`;
    
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `há ${diffHours}h`;
    
    const diffDays = Math.floor(diffHours / 24);
    return `há ${diffDays} dias`;
  }
}

