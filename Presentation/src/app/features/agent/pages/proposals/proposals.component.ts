import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
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
  showDetailsModal: boolean = false;
  showApproveModal: boolean = false;
  showRejectModal: boolean = false;
  selectedProposal: PropertyProposal | null = null;
  rejectionReason: string = '';

  constructor(
    private proposalService: ProposalService,
    private agentService: AgentService,
    private authService: AuthService,
    private toastService: ToastService,
    private http: HttpClient,
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

  openDetailsModal(proposal: PropertyProposal): void {
    this.selectedProposal = proposal;
    this.showDetailsModal = true;
  }

  closeAllModals(): void {
    this.showDetailsModal = false;
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

    if (!this.rejectionReason.trim()) {
      this.toastService.error('Por favor, indique o motivo da rejeição');
      return;
    }

    this.loading = true;
    this.proposalService.reject(this.selectedProposal.id, this.rejectionReason).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Proposta rejeitada');
          this.loadProposals();
          this.closeAllModals();
        } else {
          this.toastService.error('Erro ao rejeitar proposta');
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error rejecting proposal:', error);
        this.toastService.error('Erro ao rejeitar proposta');
        this.loading = false;
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800`;
      case 'Withdrawn':
        return `${baseClass} bg-gray-100 text-gray-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
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
}

