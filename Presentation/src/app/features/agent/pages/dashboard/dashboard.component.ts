import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { AgentService } from '../../../../core/services/agent.service';
import { PropertyService } from '../../../../core/services/property.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { VisitService } from '../../../../core/services/visit.service';
import { User } from '../../../../core/models/user.model';
import { Property, PropertyStatus } from '../../../../core/models/property.model';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { PropertyVisit } from '../../../../core/models/visit.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';
import { ThemeService } from '../../../../core/services/theme.service';
import { ToastService } from '../../../../core/services/toast.service';
import { interval, Subscription } from 'rxjs';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, AgentSidebarComponent],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class AgentDashboardComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
  agentId: string | null = null;
  myProperties: Property[] = [];
  receivedProposals: PropertyProposal[] = [];
  upcomingVisits: PropertyVisit[] = [];
  loading: boolean = true;
  currentDate: Date = new Date();
  private clockSubscription?: Subscription;

  stats = {
    totalProperties: 0,
    activeProperties: 0,
    totalProposals: 0,
    pendingProposals: 0,
    scheduledVisits: 0,
    completedVisits: 0,
    totalRevenue: 0,
    totalCommissions: 0,
    totalSales: 0,
    averageRating: 0
  };

  constructor(
    private authService: AuthService,
    private agentService: AgentService,
    private propertyService: PropertyService,
    private proposalService: ProposalService,
    private visitService: VisitService,
    private http: HttpClient,
    public themeService: ThemeService,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadAgentProfile();
    }
    
    // Update clock every minute
    this.clockSubscription = interval(60000).subscribe(() => {
      this.currentDate = new Date();
    });
  }

  ngOnDestroy(): void {
    if (this.clockSubscription) {
      this.clockSubscription.unsubscribe();
    }
  }

  loadAgentProfile(): void {
    // First get agent profile by userId to get agentId
    this.agentService.getByUserId(this.currentUser!.id).subscribe({
      next: (agent: any) => {
        this.agentId = agent.id;
        if (!agent.isActive) {
          this.toastService.warning('O seu perfil de agente está pendente de aprovação. Algumas funcionalidades podem estar limitadas.');
        }
        this.loadDashboardData();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        const errorMsg = error.error?.description || error.message || 'Erro desconhecido';
        if (error.status === 404) {
          this.toastService.error('Perfil de agente não encontrado. Por favor, contacte o suporte.');
        } else {
          this.toastService.error(`Erro ao carregar perfil do agente: ${errorMsg}`);
        }
        this.loading = false;
      }
    });
  }

  loadDashboardData(): void {
    if (!this.agentId) return;

    this.loading = true;

    // Load dashboard stats from the new endpoint
    this.agentService.getDashboardStats(this.agentId).subscribe({
      next: (stats) => {
        this.stats = stats;
        this.loadProperties();
        this.loadProposals();
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error loading dashboard stats:', error);
        // Even if stats fail, try to load other data
        this.loadProperties();
        this.loadProposals();
        this.loadVisits();
      }
    });
  }

  loadProperties(): void {
    if (!this.agentId) return;

    // Load agent's properties (últimos 5)
    this.http.get<any>(`${environment.apiUrl}/properties?agentId=${this.agentId}&pageSize=5`).subscribe({
      next: (result) => {
        if (result && result.properties) {
          this.myProperties = result.properties;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading properties:', error);
        this.loading = false;
      }
    });
  }

  loadProposals(): void {
    if (!this.agentId) return;

    // Load received proposals using the service
    this.proposalService.getByAgent(this.agentId).subscribe({
      next: (result: any) => {
        if (result && Array.isArray(result)) {
          // Direct array response
          this.receivedProposals = result.slice(0, 5);
        } else if (result && result.value && Array.isArray(result.value)) {
          // Result wrapper
          this.receivedProposals = result.value.slice(0, 5);
        }
      },
      error: (error) => {
        console.error('Error loading proposals:', error);
        this.receivedProposals = [];
      }
    });
  }

  loadVisits(): void {
    if (!this.agentId) return;

    // Load upcoming visits for the agent
    this.visitService.getVisitsByAgent(this.agentId).subscribe({
      next: (visits: PropertyVisit[]) => {
        if (visits && Array.isArray(visits)) {
          // Filter only upcoming visits (Pending or Confirmed)
          const now = new Date();
          this.upcomingVisits = visits
            .filter(v => 
              (v.status === 'Pending' || v.status === 'Confirmed') &&
              v.visitDate && new Date(v.visitDate) >= now
            )
            .sort((a, b) => {
              const dateA = a.visitDate ? new Date(a.visitDate).getTime() : 0;
              const dateB = b.visitDate ? new Date(b.visitDate).getTime() : 0;
              return dateA - dateB;
            })
            .slice(0, 5);
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading visits:', error);
        this.upcomingVisits = [];
        this.loading = false;
      }
    });
  }

  // Helper method to refresh all data
  refreshDashboard(): void {
    if (this.agentId) {
      this.loading = true;
      this.loadDashboardData();
    }
  }

  getStatusString(status: PropertyStatus | number | any): string {
    if (typeof status === 'number') {
      return PropertyStatus[status] || 'Unknown';
    }
    if (typeof status === 'string') {
      return status;
    }
    return String(status);
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Available':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Reserved':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-100 text-red-800`;
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  viewPropertyDetails(propertyId: string): void {
    // Navigate to property details page (public page)
    this.router.navigate(['/property', propertyId]);
  }
}

