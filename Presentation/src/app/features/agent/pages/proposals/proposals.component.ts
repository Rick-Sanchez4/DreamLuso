import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProposalService } from '../../../../core/services/proposal.service';
import { AgentService } from '../../../../core/services/agent.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { User } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

@Component({
  selector: 'app-agent-proposals',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent],
  templateUrl: './proposals.component.html',
  styleUrl: './proposals.component.scss'
})
export class AgentProposalsComponent implements OnInit {
  currentUser: User | null = null;
  agentId: string | null = null;
  proposals: PropertyProposal[] = [];
  filteredProposals: PropertyProposal[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  searchTerm: string = '';

  // Modals
  showApproveModal: boolean = false;
  showRejectModal: boolean = false;
  selectedProposal: PropertyProposal | null = null;
  rejectionReason: string = '';

  // Negotiation (per proposal)
  negotiationMessages: Map<string, string> = new Map();
  counterOffers: Map<string, number | undefined> = new Map();

  constructor(
    private proposalService: ProposalService,
    private agentService: AgentService,
    private authService: AuthService,
    private toastService: ToastService,
    private http: HttpClient,
    public themeService: ThemeService,
    private route: ActivatedRoute,
    private router: Router
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
        this.loadProposals();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        this.toastService.error('Erro ao carregar perfil do agente');
        this.loading = false;
      }
    });
  }

  loadProposals(): void {
    if (!this.agentId) return;
    
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/proposals/agent/${this.agentId}`).subscribe({
      next: (proposals) => {
        if (proposals && Array.isArray(proposals)) {
          this.proposals = proposals;
          this.applyFilters();
          
          // If there's a proposal ID in the route, remove it (no modal to open)
          const proposalId = this.route.snapshot.paramMap.get('id');
          if (proposalId) {
            this.router.navigate(['/agent/proposals'], { replaceUrl: true });
          }
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading proposals:', error);
        this.toastService.error('Erro ao carregar propostas');
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredProposals = this.proposals.filter(p => {
      const matchesStatus = this.statusFilter === 'all' || p.status === this.statusFilter;
      const matchesSearch = !this.searchTerm || 
        p.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.proposalNumber.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  openApproveModal(proposal: PropertyProposal): void {
    this.selectedProposal = proposal;
    this.showApproveModal = true;
  }

  openRejectModal(proposal: PropertyProposal): void {
    this.selectedProposal = proposal;
    this.rejectionReason = '';
    this.showRejectModal = true;
  }

  closeAllModals(): void {
    this.showApproveModal = false;
    this.showRejectModal = false;
    this.selectedProposal = null;
    this.rejectionReason = '';
  }

  confirmApprove(): void {
    if (!this.selectedProposal) return;

    this.loading = true;
    this.proposalService.approve(this.selectedProposal.id).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Proposta aprovada com sucesso!');
          this.loadProposals();
          this.closeAllModals();
        } else {
          this.toastService.error('Erro ao aprovar proposta');
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error approving proposal:', error);
        this.toastService.error('Erro ao aprovar proposta');
        this.loading = false;
      }
    });
  }

  confirmReject(): void {
    if (!this.selectedProposal) return;

    const trimmedReason = this.rejectionReason.trim();
    
    if (!trimmedReason) {
      this.toastService.error('Por favor, indique o motivo da rejeição');
      return;
    }

    // Backend requires minimum 10 characters
    if (trimmedReason.length < 10) {
      this.toastService.error('O motivo da rejeição deve ter pelo menos 10 caracteres');
      return;
    }

    // Backend has maximum 500 characters
    if (trimmedReason.length > 500) {
      this.toastService.error('O motivo da rejeição não pode exceder 500 caracteres');
      return;
    }

    this.loading = true;
    this.proposalService.reject(this.selectedProposal.id, trimmedReason).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Proposta rejeitada');
          this.loadProposals();
          this.closeAllModals();
        } else {
          const errorMsg = result.error?.description || 'Erro ao rejeitar proposta';
          this.toastService.error(errorMsg);
        }
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error rejecting proposal:', error);
        let errorMsg = 'Erro ao rejeitar proposta';
        
        // Handle validation errors from backend
        if (error.error?.errors) {
          const validationErrors = error.error.errors;
          const errorMessages: string[] = [];
          Object.keys(validationErrors).forEach(key => {
            const errors = validationErrors[key] as string[];
            if (errors && errors.length > 0) {
              errorMessages.push(errors.join(', '));
            }
          });
          errorMsg = errorMessages.length > 0 ? errorMessages.join(' | ') : errorMsg;
        } else if (error.error?.description) {
          errorMsg = error.error.description;
        } else if (error.error?.message) {
          errorMsg = error.error.message;
        }
        
        this.toastService.error(errorMsg);
        this.loading = false;
      }
    });
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending':
        return 'Pendente';
      case 'UnderAnalysis':
        return 'Em Análise';
      case 'InNegotiation':
        return 'Em Negociação';
      case 'Approved':
        return 'Aprovada';
      case 'Rejected':
        return 'Rejeitada';
      case 'Cancelled':
        return 'Cancelada';
      case 'Completed':
        return 'Concluída';
      default:
        return status;
    }
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400`;
      case 'Cancelled':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
      case 'Completed':
        return `${baseClass} bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
    }
  }

  getTypeBadgeClass(type: string): string {
    return type === 'Purchase' 
      ? 'px-3 py-1 rounded-full text-xs font-semibold bg-blue-100 text-blue-800'
      : 'px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800';
  }

  getStatusIcon(status: string): string {
    switch (status) {
      case 'Pending':
        return 'M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'Approved':
        return 'M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z';
      case 'Rejected':
        return 'M10 14l2-2m0 0l2-2m-2 2l-2-2m2 2l2 2m7-2a9 9 0 11-18 0 9 9 0 0118 0z';
      default:
        return 'M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z';
    }
  }

  updateNegotiationStatus(negotiationId: string, event: Event): void {
    const selectElement = event.target as HTMLSelectElement;
    const newStatus = selectElement.value;

    this.proposalService.updateNegotiationStatus(negotiationId, newStatus).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Estado da negociação atualizado');
          this.loadProposals();
        } else {
          const errorMsg = result.error?.description || 'Erro ao atualizar estado da negociação';
          this.toastService.error(errorMsg);
          // Revert the select value on error
          this.loadProposals();
        }
      },
      error: (error: any) => {
        console.error('Error updating negotiation status:', error);
        this.toastService.error('Erro ao atualizar estado da negociação');
        // Revert the select value on error
        this.loadProposals();
      }
    });
  }

  getNegotiationStatusBadgeClass(status: string): string {
    const baseClass = 'px-2 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Sent':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300`;
      case 'Viewed':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300`;
      case 'Accepted':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300`;
    }
  }

  getNegotiationStatusLabel(status: string): string {
    switch (status) {
      case 'Sent':
        return 'Enviado';
      case 'Viewed':
        return 'Visualizado';
      case 'Accepted':
        return 'Aceito';
      case 'Rejected':
        return 'Rejeitado';
      default:
        return status;
    }
  }

  sendNegotiation(proposalId: string): void {
    const message = this.negotiationMessages.get(proposalId) || '';
    const counterOffer = this.counterOffers.get(proposalId);
    
    if (!message.trim()) {
      this.toastService.error('Por favor, preencha a mensagem');
      return;
    }
    
    const userId = this.currentUser?.id || '';
    if (!userId) {
      this.toastService.error('Utilizador não encontrado');
      return;
    }
    
    // Validar mensagem (mínimo 5 caracteres conforme backend)
    const trimmedMessage = message.trim();
    if (trimmedMessage.length < 5) {
      this.toastService.error('A mensagem deve ter pelo menos 5 caracteres');
      return;
    }
    
    // Validar contraproposta se fornecida
    if (counterOffer !== undefined && counterOffer !== null && counterOffer <= 0) {
      this.toastService.error('A contraproposta deve ser maior que zero');
      return;
    }
    
    this.proposalService
      .addNegotiation(proposalId, userId, trimmedMessage, counterOffer)
      .subscribe({
        next: (result: any) => {
          if (result.isSuccess) {
            this.toastService.success('Negociação enviada com sucesso!');
            this.negotiationMessages.delete(proposalId);
            this.counterOffers.delete(proposalId);
            this.loadProposals();
          } else {
            const errorMsg = result.error?.description || 'Erro ao adicionar negociação';
            this.toastService.error(errorMsg);
          }
        },
        error: (error: any) => {
          console.error('Erro ao adicionar negociação:', error);
          const errorMsg = error.error?.description || error.error?.message || 'Erro ao adicionar negociação';
          this.toastService.error(errorMsg);
        }
      });
  }

  getNegotiationMessage(proposalId: string): string {
    return this.negotiationMessages.get(proposalId) || '';
  }

  setNegotiationMessage(proposalId: string, message: string): void {
    this.negotiationMessages.set(proposalId, message);
  }

  getCounterOffer(proposalId: string): number | undefined {
    return this.counterOffers.get(proposalId);
  }

  setCounterOffer(proposalId: string, value: number | undefined): void {
    this.counterOffers.set(proposalId, value);
  }

  onCounterOfferChange(proposalId: string, event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value ? parseFloat(input.value) : undefined;
    this.setCounterOffer(proposalId, value);
  }

  startAnalysis(proposalId: string): void {
    this.proposalService.startAnalysis(proposalId).subscribe({
      next: (result: any) => {
        if (result.isSuccess) {
          this.toastService.success('Análise iniciada com sucesso!');
          this.loadProposals();
        } else {
          const errorMsg = result.error?.description || 'Erro ao iniciar análise';
          this.toastService.error(errorMsg);
        }
      },
      error: (error: any) => {
        console.error('Erro ao iniciar análise:', error);
        const errorMsg = error.error?.description || error.error?.message || 'Erro ao iniciar análise';
        this.toastService.error(errorMsg);
      }
    });
  }
}

