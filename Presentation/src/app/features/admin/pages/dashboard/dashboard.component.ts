import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { User } from '../../../../core/models/user.model';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { ThemeService } from '../../../../core/services/theme.service';

interface AdminStats {
  totalUsers: number;
  totalClients: number;
  totalAgents: number;
  totalProperties: number;
  activeProperties: number;
  totalProposals: number;
  pendingProposals: number;
  totalContracts: number;
  monthlyRevenue: number;
  averageContractValue: number;
  conversionRate: number;
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  currentUser: User | null = null;
  currentDate: Date = new Date();
  loading: boolean = true;

  stats: AdminStats = {
    totalUsers: 0,
    totalClients: 0,
    totalAgents: 0,
    totalProperties: 0,
    activeProperties: 0,
    totalProposals: 0,
    pendingProposals: 0,
    totalContracts: 0,
    monthlyRevenue: 0,
    averageContractValue: 0,
    conversionRate: 0
  };

  recentActivity: any[] = [];

  constructor(
    private authService: AuthService,
    private http: HttpClient,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadDashboardData();
    }
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load admin statistics from the real endpoint
    this.http.get<AdminStats>(`${environment.apiUrl}/dashboard/admin`).subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        this.loading = false;
      }
    });
  }

  getChangeIndicator(value: number): string {
    return value >= 0 ? 'positive' : 'negative';
  }
}

