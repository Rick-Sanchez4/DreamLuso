import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProposalService } from '../../../../core/services/proposal.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-agent-proposals',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './proposals.component.html',
  styleUrl: './proposals.component.scss'
})
export class AgentProposalsComponent implements OnInit {
  currentUser: User | null = null;
  proposals: PropertyProposal[] = [];
  filteredProposals: PropertyProposal[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private proposalService: ProposalService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadProposals();
    }
  }

  loadProposals(): void {
    this.loading = true;
    this.proposalService.getByAgent(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.proposals = result.value;
        this.applyFilters();
      } else {
        this.toastService.error('Erro ao carregar propostas');
      }
      this.loading = false;
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

  approveProposal(proposalId: string): void {
    if (!confirm('Tem certeza que deseja aprovar esta proposta?')) {
      return;
    }

    this.proposalService.approve(proposalId).subscribe(result => {
      if (result.isSuccess) {
        this.toastService.success('Proposta aprovada com sucesso!');
        this.loadProposals();
      } else {
        this.toastService.error('Erro ao aprovar proposta');
      }
    });
  }

  rejectProposal(proposalId: string): void {
    const reason = prompt('Motivo da rejeição (opcional):');
    if (reason === null) return; // User cancelled

    this.proposalService.reject(proposalId, reason || 'Sem motivo especificado').subscribe(result => {
      if (result.isSuccess) {
        this.toastService.success('Proposta rejeitada');
        this.loadProposals();
      } else {
        this.toastService.error('Erro ao rejeitar proposta');
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

