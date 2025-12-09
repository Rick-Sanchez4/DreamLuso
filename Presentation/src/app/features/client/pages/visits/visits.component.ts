import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VisitService } from '../../../../core/services/visit.service';
import { ClientService } from '../../../../core/services/client.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { PropertyVisit, VisitStatus } from '../../../../core/models/visit.model';
import { User } from '../../../../core/models/user.model';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';

@Component({
  selector: 'app-client-visits',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ClientSidebarComponent],
  templateUrl: './visits.component.html',
  styleUrl: './visits.component.scss'
})
export class ClientVisitsComponent implements OnInit {
  currentUser: User | null = null;
  clientId: string | null = null;
  visits: PropertyVisit[] = [];
  filteredVisits: PropertyVisit[] = [];
  loading: boolean = true;

  statusFilter: string = 'all';
  searchTerm: string = '';

  showCancelModal: boolean = false;
  selectedVisitId: string | null = null;
  cancellationReason: string = '';

  constructor(
    private visitService: VisitService,
    private authService: AuthService,
    private clientService: ClientService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.clientService.getByUserId(this.currentUser.id).subscribe({
        next: (client: any) => {
          this.clientId = client?.id || null;
          this.loadVisits();
        },
        error: () => {
          this.loading = false;
        }
      });
    }
  }

  loadVisits(): void {
    if (!this.clientId) return;
    this.loading = true;
    this.visitService.getVisitsByClient(this.clientId).subscribe({
      next: (visits: PropertyVisit[]) => {
        this.visits = Array.isArray(visits) ? visits : [];
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.visits = [];
        this.filteredVisits = [];
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredVisits = this.visits.filter(v => {
      const matchesStatus = this.statusFilter === 'all' || v.status === this.statusFilter as VisitStatus;
      const matchesSearch = !this.searchTerm ||
        v.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        v.agentName?.toLowerCase().includes(this.searchTerm.toLowerCase());
      return matchesStatus && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
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

  isUpcoming(visit: PropertyVisit): boolean {
    if (!visit.visitDate) return false;
    const visitDate = new Date(visit.visitDate);
    return visitDate >= new Date();
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

    this.visitService.cancelVisit({
      visitId: this.selectedVisitId,
      reason: this.cancellationReason.trim()
    }).subscribe({
      next: () => {
        this.toastService.success('Visita cancelada com sucesso');
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
}


