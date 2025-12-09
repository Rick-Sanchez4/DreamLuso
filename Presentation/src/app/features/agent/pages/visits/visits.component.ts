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

    // Sort by visit date (closest first)
    this.filteredVisits.sort((a, b) => {
      const dateA = a.visitDate ? new Date(a.visitDate).getTime() : 0;
      const dateB = b.visitDate ? new Date(b.visitDate).getTime() : 0;
      return dateA - dateB;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  showConfirmModal: boolean = false;
  showCancelModal: boolean = false;
  selectedVisitId: string | null = null;
  cancellationReason: string = '';

  confirmVisit(visitId: string): void {
    this.selectedVisitId = visitId;
    this.showConfirmModal = true;
  }

  closeConfirmModal(): void {
    this.showConfirmModal = false;
    this.selectedVisitId = null;
  }

  confirmVisitAction(): void {
    if (!this.selectedVisitId) return;

    this.visitService.confirmVisit({ visitId: this.selectedVisitId }).subscribe({
      next: () => {
        this.toastService.success('Visita confirmada com sucesso!');
        this.closeConfirmModal();
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error confirming visit:', error);
        const errorMsg = error.error?.description || error.error?.message || 'Erro ao confirmar visita';
        this.toastService.error(errorMsg);
      }
    });
  }

  cancelVisit(visitId: string): void {
    this.selectedVisitId = visitId;
    this.cancellationReason = '';
    this.showCancelModal = true;
  }

  closeCancelModal(): void {
    this.showCancelModal = false;
    this.selectedVisitId = null;
    this.cancellationReason = '';
  }

  confirmCancelVisit(): void {
    if (!this.selectedVisitId || !this.cancellationReason.trim()) {
      this.toastService.warning('Por favor, forneça um motivo para o cancelamento');
      return;
    }

    this.visitService.cancelVisit({ visitId: this.selectedVisitId, reason: this.cancellationReason.trim() }).subscribe({
      next: () => {
        this.toastService.success('Visita cancelada');
        this.closeCancelModal();
        this.loadVisits();
      },
      error: (error) => {
        console.error('Error cancelling visit:', error);
        const errorMsg = error.error?.description || error.error?.message || 'Erro ao cancelar visita';
        this.toastService.error(errorMsg);
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
      case 'Rescheduled':
        return `${baseClass} bg-purple-100 text-purple-800 dark:bg-purple-900/20 dark:text-purple-400`;
      case 'NoShow':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400`;
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending': return 'Pendente';
      case 'Confirmed': return 'Confirmada';
      case 'Completed': return 'Concluída';
      case 'Cancelled': return 'Cancelada';
      case 'Rescheduled': return 'Reagendada';
      case 'NoShow': return 'Faltou';
      default: return status;
    }
  }

  getTimeSlotLabel(timeSlot: string): string {
    switch (timeSlot) {
      case 'Morning_9AM_11AM': return '09:00 - 11:00';
      case 'Morning_11AM_1PM': return '11:00 - 13:00';
      case 'Afternoon_2PM_4PM': return '14:00 - 16:00';
      case 'Afternoon_4PM_6PM': return '16:00 - 18:00';
      case 'Evening_6PM_8PM': return '18:00 - 20:00';
      default: return timeSlot;
    }
  }

  formatVisitDateTime(visit: PropertyVisit): string {
    if (!visit.visitDate) return '';
    const date = new Date(visit.visitDate);
    const timeSlot = this.getTimeSlotLabel(visit.timeSlot || '');
    return `${date.toLocaleDateString('pt-PT')} às ${timeSlot}`;
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
    if (!visit.visitDate) return false;
    const visitDate = new Date(visit.visitDate);
    return visitDate >= new Date();
  }

  isPast(visit: PropertyVisit): boolean {
    if (!visit.visitDate) return false;
    const visitDate = new Date(visit.visitDate);
    return visitDate < new Date();
  }
}

