import { Component, OnInit, OnDestroy, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, NavigationEnd } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { NotificationService } from '../../../core/services/notification.service';
import { ThemeService } from '../../../core/services/theme.service';
import { User, UserRole } from '../../../core/models/user.model';
import { Notification, NotificationStatus } from '../../../core/models/notification.model';
import { filter, Subscription } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.scss'
})
export class NavbarComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
  notifications: Notification[] = [];
  unreadCount: number = 0;
  showNotifications: boolean = false;
  isPublicRoute: boolean = true;
  private subscriptions = new Subscription();
  private refreshNotificationsHandler = () => this.loadNotifications();

  constructor(
    private authService: AuthService,
    private notificationService: NotificationService,
    public themeService: ThemeService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Determine initial route context
    this.updateRouteContext();
    
    // Listen for custom events to refresh notifications
    window.addEventListener('notifications:refresh', this.refreshNotificationsHandler);

    // Update on navigation - only listen to NavigationEnd to avoid excessive updates
    const routerSub = this.router.events
      .pipe(filter((event: any) => event instanceof NavigationEnd))
      .subscribe(() => {
        this.updateRouteContext();
        // Reload notifications when navigating to a new route
        if (this.currentUser && this.authService.isAuthenticated()) {
          this.loadNotifications();
        }
      });
    this.subscriptions.add(routerSub);

    // Subscribe to user changes
    const userSub = this.authService.currentUser$.subscribe((user: User | null) => {
      this.currentUser = user;
      if (user && this.authService.isAuthenticated()) {
        // Small delay to ensure route is updated
        setTimeout(() => {
          this.updateRouteContext();
          this.loadNotifications();
        }, 100);
      } else {
        // Clear notifications if user is not authenticated
        this.notifications = [];
        this.unreadCount = 0;
      }
    });
    this.subscriptions.add(userSub);

    // Subscribe to unread count changes
    const countSub = this.notificationService.unreadCount$.subscribe((count: number) => {
      this.unreadCount = count;
    });
    this.subscriptions.add(countSub);
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    // Remove event listener
    window.removeEventListener('notifications:refresh', this.refreshNotificationsHandler);
  }

  private updateRouteContext(): void {
    const url = this.router.url;
    this.isPublicRoute = this.checkIsPublic(url);
  }

  loadNotifications(): void {
    // Load notifications if user is authenticated (regardless of route)
    // The UI will hide notifications on public routes, but we still load them
    if (this.currentUser && this.authService.isAuthenticated()) {
      this.notificationService.getUserNotifications(this.currentUser.id).subscribe({
        next: (result: any) => {
          if (result.isSuccess && result.value) {
            this.notifications = result.value;
            // Update unread count from notifications - use proper status check
            this.unreadCount = this.getUnreadCount(result.value);
          } else {
            this.notifications = [];
            this.unreadCount = 0;
          }
        },
        error: (error: any) => {
          // Silently handle errors - notifications are optional
          // Backend might not be running (status 0 = ERR_CONNECTION_REFUSED)
          if (error.status === 0) {
            console.warn('Backend não está disponível. Notificações não serão carregadas.');
          }
          this.notifications = [];
          this.unreadCount = 0;
        }
      });

      // Also get unread count separately for real-time updates
      this.notificationService.getUnreadCount(this.currentUser.id).subscribe({
        next: (result: any) => {
          if (result.isSuccess && result.value) {
            this.unreadCount = result.value.unreadCount || 0;
          }
        },
        error: (error: any) => {
          // Silently handle errors - backend might not be running
          // Status 0 = ERR_CONNECTION_REFUSED
          if (error.status === 0) {
            console.warn('Backend não está disponível. Contador de notificações não será atualizado.');
          }
          this.unreadCount = 0;
        }
      });
    } else {
      this.notifications = [];
      this.unreadCount = 0;
    }
  }

  private getUnreadCount(notifications: Notification[]): number {
    if (!notifications || notifications.length === 0) {
      return 0;
    }
    
    return notifications.filter(n => {
      // Check status using enum or string comparison (case-insensitive)
      const status = n.status?.toString() || '';
      return status.toLowerCase() === NotificationStatus.Unread.toLowerCase() || 
             status === NotificationStatus.Unread ||
             status === 'Unread';
    }).length;
  }

  toggleNotifications(): void {
    this.showNotifications = !this.showNotifications;
    // If opening, ensure notifications are loaded
    if (this.showNotifications && this.currentUser && this.notifications.length === 0) {
      this.loadNotifications();
    }
  }

  handleNotificationClick(notification: Notification): void {
    // Only mark as read if it's currently unread
    const isUnread = this.isUnread(notification);
    
    if (isUnread) {
      this.notificationService.markAsRead(notification.id).subscribe({
        next: () => {
          // Update notification status locally for immediate feedback
          notification.status = NotificationStatus.Read;
          // Recalculate unread count
          this.unreadCount = this.getUnreadCount(this.notifications);
          // Reload to ensure sync with backend
          this.loadNotifications();
        },
        error: (error: any) => {
          console.error('Error marking notification as read:', error);
          // Still navigate even if marking as read fails
        }
      });
    }

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
      this.notificationService.markAllAsRead(this.currentUser.id).subscribe({
        next: () => {
          // Update all notifications locally for immediate feedback
          this.notifications.forEach(n => {
            if (this.isUnread(n)) {
              n.status = NotificationStatus.Read;
            }
          });
          this.unreadCount = 0;
          // Reload to ensure sync with backend
          this.loadNotifications();
        },
        error: (error: any) => {
          console.error('Error marking all notifications as read:', error);
        }
      });
    }
  }

  isUnread(notification: Notification): boolean {
    if (!notification || !notification.status) {
      return false;
    }
    
    const status = notification.status.toString();
    return status.toLowerCase() === NotificationStatus.Unread.toLowerCase() || 
           status === NotificationStatus.Unread ||
           status === 'Unread';
  }

  navigateTo(path: string, queryParams?: { [key: string]: any }): void {
    if (queryParams) {
      this.router.navigate([path], { queryParams });
    } else {
      this.router.navigate([path]);
    }
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

  private checkIsPublic(url: string): boolean {
    // Consider admin/agent/client areas as private; rest is public
    // Also check for routes that start with these paths
    const isPrivate = /^\/(admin|agent|client)(\/|$)/.test(url);
    return !isPrivate;
  }

  getNotificationClass(notification: Notification): string {
    const baseClasses = 'transition-all duration-200';
    if (this.isUnread(notification)) {
      // Notificações não lidas: fundo mais visível e destacado
      return `${baseClasses} bg-green-50 dark:bg-green-900/30 border-l-4 border-green-500 shadow-sm`;
    } else {
      // Notificações lidas: fundo escuro/neutro
      return `${baseClasses} bg-gray-100 dark:bg-dark-700/50 border-l-4 border-gray-300 dark:border-gray-600 opacity-75`;
    }
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

  getImageUrl(profileImageUrl: string | undefined): string {
    if (!profileImageUrl || profileImageUrl.trim() === '') {
      return this.getDefaultProfileImageUrl();
    }

    // Se já é uma URL completa (http/https), retornar como está
    if (profileImageUrl.startsWith('http')) {
      return profileImageUrl;
    }

    // Se começa com /, é um caminho relativo para arquivos estáticos
    if (profileImageUrl.startsWith('/')) {
      const baseUrl = environment.baseUrl || environment.apiUrl.replace('/api', '');
      return `${baseUrl}${profileImageUrl}`;
    }

    // Caso contrário, assumir que é apenas o nome do arquivo
    const baseUrl = environment.baseUrl || environment.apiUrl.replace('/api', '');
    return `${baseUrl}/images/profiles/${profileImageUrl}`;
  }

  getDefaultProfileImageUrl(): string {
    // Retornar uma imagem padrão SVG inline ou usar um placeholder
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAiIGhlaWdodD0iNDAiIHZpZXdCb3g9IjAgMCA0MCA0MCIgZmlsbD0ibm9uZSIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KPGNpcmNsZSBjeD0iMjAiIGN5PSIyMCIgcj0iMjAiIGZpbGw9IiMxMEI5ODEiLz4KPHN2ZyB4PSIxMCIgeT0iMTAiIHdpZHRoPSIyMCIgaGVpZ2h0PSIyMCIgdmlld0JveD0iMCAwIDI0IDI0IiBmaWxsPSJub25lIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPgo8cGF0aCBkPSJNMTIgMTJDMTQuMjA5MSAxMiAxNiAxMC4yMDkxIDE2IDhDMTYgNS43OTA4NiAxNC4yMDkxIDQgMTIgNEM5Ljc5MDg2IDQgOCA1Ljc5MDg2IDggOEM4IDEwLjIwOTEgOS43OTA4NiAxMiAxMiAxMloiIGZpbGw9IndoaXRlIi8+CjxwYXRoIGQ9Ik0xMiAxNEMxNS4zMTM3IDE0IDE4IDExLjMxMzcgMTggOEMxOCA0LjY4NjMgMTUuMzEzNyAyIDEyIDJDOC42ODYzIDIgNiA0LjY4NjMgNiA4QzYgMTEuMzEzNyA4LjY4NjMgMTQgMTIgMTRaIiBmaWxsPSJ3aGl0ZSIvPgo8L3N2Zz4KPC9zdmc+';
  }

  handleImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    // Prevenir loop infinito se a imagem padrão também falhar
    if (img.src === this.getDefaultProfileImageUrl() || img.src.includes('data:image/svg')) {
      return;
    }
    img.src = this.getDefaultProfileImageUrl();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    // Close notifications dropdown if clicking outside
    const target = event.target as HTMLElement;
    const container = target.closest('.notification-dropdown-container');
    if (this.showNotifications && !container) {
      this.showNotifications = false;
    }
  }
}

