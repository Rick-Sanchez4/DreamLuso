import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ClientService } from '../../../../core/services/client.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { User } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';
import { ThemeService } from '../../../../core/services/theme.service';

@Component({
  selector: 'app-client-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, ClientSidebarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class ClientDashboardComponent implements OnInit {
  currentUser: User | null = null;
  clientId: string | null = null;
  myProposals: PropertyProposal[] = [];
  unreadNotifications: number = 0;
  loading: boolean = true;

  stats = {
    totalProposals: 0,
    pendingProposals: 0,
    approvedProposals: 0,
    rejectedProposals: 0,
    totalVisits: 0,
    scheduledVisits: 0,
    completedVisits: 0,
    totalFavorites: 0,
    totalContracts: 0,
    activeContracts: 0
  };

  constructor(
    private authService: AuthService,
    private clientService: ClientService,
    private proposalService: ProposalService,
    private notificationService: NotificationService,
    private http: HttpClient,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadClientProfile();
    }
  }

  loadClientProfile(): void {
    this.clientService.getByUserId(this.currentUser!.id).subscribe({
      next: (client: any) => {
        if (client && client.id) {
          this.clientId = client.id;
          this.loadDashboardData();
        } else {
          console.error('Client profile not found or invalid');
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error loading client profile:', error);
        // If 404, try to create the client profile automatically
        if (error.status === 404) {
          console.warn('Client profile not found. Creating automatically...');
          this.createClientProfile();
        } else {
          this.loading = false;
        }
      }
    });
  }

  createClientProfile(): void {
    // Create client profile with default values
    const createRequest = {
      userId: this.currentUser!.id,
      type: 0, // Individual (ClientType.Individual = 0)
      nif: null,
      citizenCard: null,
      minBudget: null,
      maxBudget: null,
      preferredContactMethod: null
    };

    this.http.post<any>(`${environment.apiUrl}/clients`, createRequest).subscribe({
      next: (response: any) => {
        if (response && (response.clientId || response.id)) {
          this.clientId = response.clientId || response.id;
          console.log('Client profile created successfully');
          this.loadDashboardData();
        } else {
          console.error('Failed to create client profile');
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error creating client profile:', error);
        this.loading = false;
      }
    });
  }

  loadDashboardData(): void {
    if (!this.clientId) return;

    this.loading = true;

    // Load dashboard stats from the new endpoint
    this.clientService.getDashboardStats(this.clientId).subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loadProposals();
        this.loadNotifications();
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        this.loading = false;
      }
    });
  }

  loadProposals(): void {
    if (!this.clientId) return;

    // Load client's proposals (últimos 5) - usar o serviço para consistência
    this.proposalService.getByClient(this.clientId).subscribe({
      next: (result) => {
        if (result.isSuccess && result.value) {
          this.myProposals = result.value.slice(0, 5);
        } else {
          this.myProposals = [];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading proposals:', error);
        this.myProposals = [];
        this.loading = false;
      }
    });
  }

  loadNotifications(): void {
    // Load unread notifications
    this.notificationService.getUnreadCount(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.unreadNotifications = result.value.unreadCount;
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-300`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300`;
      case 'Cancelled':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
      case 'Completed':
        return `${baseClass} bg-emerald-100 text-emerald-800 dark:bg-emerald-900/30 dark:text-emerald-300`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
    }
  }
}

