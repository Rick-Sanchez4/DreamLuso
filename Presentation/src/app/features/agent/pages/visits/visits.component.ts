import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VisitService } from '../../../../core/services/visit.service';
import { AgentService } from '../../../../core/services/agent.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { PropertyVisit } from '../../../../core/models/visit.model';
import { User } from '../../../../core/models/user.model';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

@Component({
  selector: 'app-agent-visits',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent],
  templateUrl: './visits.component.html',
  styleUrl: './visits.component.scss'
})
export class AgentVisitsComponent implements OnInit {
  currentUser: User | null = null;
  agentId: string | null = null;
  visits: PropertyVisit[] = [];
  filteredVisits: PropertyVisit[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private visitService: VisitService,
    private agentService: AgentService,
    private authService: AuthService,
    private toastService: ToastService,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadAgentProfile();
    }
  }

  loadAgentProfile(): void {
    this.agentService.getByUserId(this.currentUser!.id).subscribe({
      next: (agent: any) => {
        this.agentId = agent.id;
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        this.toastService.error('Erro ao carregar perfil do agente');
        this.loading = false;
      }
    });
  }

  loadVisits(): void {
    if (!this.agentId) return;
    
    this.loading = true;
    this.visitService.getVisitsByAgent(this.agentId).subscribe({
      next: (visits) => {
        if (visits && Array.isArray(visits)) {
          this.visits = visits;
          this.applyFilters();
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading visits:', error);
        this.toastService.error('Erro ao carregar visitas');
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredVisits = this.visits.filter(v => {
      const matchesStatus = this.statusFilter === 'all' || v.status === this.statusFilter;
      const matchesSearch = !this.searchTerm || 
        v.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        v.clientName?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesSearch;
    });

    // Sort by scheduled date (closest first)
    this.filteredVisits.sort((a, b) => 
      new Date(a.scheduledDate).getTime() - new Date(b.scheduledDate).getTime()
    );
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  confirmVisit(visitId: string): void {
    if (!confirm('Tem certeza que deseja confirmar esta visita?')) {
      return;
    }

    this.visitService.confirmVisit({ visitId }).subscribe({
      next: () => {
        this.toastService.success('Visita confirmada com sucesso!');
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error confirming visit:', error);
        this.toastService.error('Erro ao confirmar visita');
      }
    });
  }

  cancelVisit(visitId: string): void {
    const reason = prompt('Motivo do cancelamento:');
    if (reason === null) return;

    this.visitService.cancelVisit({ visitId, reason }).subscribe({
      next: () => {
        this.toastService.success('Visita cancelada');
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error cancelling visit:', error);
        this.toastService.error('Erro ao cancelar visita');
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400`;
      case 'Confirmed':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/20 dark:text-blue-400`;
      case 'Completed':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400`;
      case 'Cancelled':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400`;
      case 'NoShow':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Pending':
        return 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'Confirmed':
        return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'Completed':
        return 'M5 13l4 4L19 7';
      case 'Cancelled':
        return 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z';
      default:
        return 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
    }
  }

  isUpcoming(visit: PropertyVisit): boolean {
    return new Date(visit.scheduledDate) >= new Date();
  }

  isPast(visit: PropertyVisit): boolean {
    return new Date(visit.scheduledDate) < new Date();
  }

  formatDate(date: Date): string {
    const visitDate = new Date(date);
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);

    if (visitDate.toDateString() === today.toDateString()) {
      return 'Hoje';
    } else if (visitDate.toDateString() === tomorrow.toDateString()) {
      return 'Amanhã';
    } else {
      return visitDate.toLocaleDateString('pt-PT', { weekday: 'long', day: 'numeric', month: 'long' });
    }
  }
}

