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
    monthlyRevenue: 0
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

    // Load admin statistics
    this.http.get<any>(`${environment.apiUrl}/dashboard/admin`).subscribe({
      next: (result) => {
        if (result.isSuccess && result.value) {
          this.stats = result.value;
        }
        this.loading = false;
      },
      error: () => {
        // Mock data for development
        this.stats = {
          totalUsers: 156,
          totalClients: 98,
          totalAgents: 12,
          totalProperties: 234,
          activeProperties: 187,
          totalProposals: 45,
          pendingProposals: 12,
          totalContracts: 89,
          monthlyRevenue: 125000
        };
        this.loading = false;
      }
    });
  }

  getChangeIndicator(value: number): string {
    return value >= 0 ? 'positive' : 'negative';
  }
}

