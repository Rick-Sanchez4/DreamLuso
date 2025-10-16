import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { environment } from '../../../../../environments/environment';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-admin-proposals',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent, FormsModule],
  templateUrl: './proposals.component.html',
  styleUrl: './proposals.component.scss'
})
export class AdminProposalsComponent implements OnInit {
  proposals: any[] = [];
  filteredProposals: any[] = [];
  loading: boolean = true;
  statusFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private http: HttpClient,
    private toastService: ToastService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProposals();
  }

  loadProposals(): void {
    this.loading = true;
    
    // Get all agents to fetch their proposals
    this.http.get<any>(`${environment.apiUrl}/agents?pageSize=100`).subscribe({
      next: (agentsResult) => {
        const agents = agentsResult.agents || [];
        let allProposals: any[] = [];
        let completed = 0;
        
        if (agents.length === 0) {
          this.proposals = [];
          this.applyFilters();
          this.loading = false;
          return;
        }
        
        // Fetch proposals for each agent
        agents.forEach((agent: any, index: number) => {
          this.http.get<any>(`${environment.apiUrl}/proposals/agent/${agent.id}`).subscribe({
            next: (agentProposals) => {
              if (Array.isArray(agentProposals)) {
                allProposals = allProposals.concat(agentProposals);
              }
              completed++;
              
              if (completed === agents.length) {
                this.proposals = allProposals;
                this.applyFilters();
                this.loading = false;
              }
            },
            error: () => {
              completed++;
              if (completed === agents.length) {
                this.proposals = allProposals;
                this.applyFilters();
                this.loading = false;
              }
            }
          });
        });
      },
      error: () => {
        this.proposals = [];
        this.applyFilters();
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredProposals = this.proposals.filter(p => {
      const matchesStatus = this.statusFilter === 'all' || p.status === this.statusFilter;
      const matchesSearch = !this.searchTerm || 
        p.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.clientName?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-bold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
    }
  }

  getPendingCount(): number {
    return this.proposals.filter(p => p.status === 'Pending' || p.status === 'UnderAnalysis').length;
  }

  getApprovedCount(): number {
    return this.proposals.filter(p => p.status === 'Approved').length;
  }

  getRejectedCount(): number {
    return this.proposals.filter(p => p.status === 'Rejected').length;
  }

  getTotalValue(): number {
    return this.filteredProposals.reduce((sum, p) => sum + (p.proposedValue || 0), 0);
  }

  viewProposal(proposal: any): void {
    // Navigate to property page (same tab)
    this.router.navigate(['/property', proposal.propertyId]);
  }

  approveProposal(proposal: any): void {
    if (!confirm(`Aprovar proposta de €${proposal.proposedValue.toLocaleString('pt-PT')} para "${proposal.propertyTitle}"?`)) {
      return;
    }

    this.http.put(`${environment.apiUrl}/proposals/${proposal.id}/approve`, {}).subscribe({
      next: () => {
        this.toastService.success('Proposta aprovada com sucesso!');
        this.loadProposals();
      },
      error: (error) => {
        console.error('Error approving proposal:', error);
        this.toastService.error('Erro ao aprovar proposta');
      }
    });
  }

  rejectProposal(proposal: any): void {
    const reason = prompt('Motivo da rejeição (opcional):');
    if (reason === null) return; // User cancelled

    this.http.put(`${environment.apiUrl}/proposals/${proposal.id}/reject`, {
      proposalId: proposal.id,
      reason: reason || 'Não especificado'
    }).subscribe({
      next: () => {
        this.toastService.success('Proposta rejeitada!');
        this.loadProposals();
      },
      error: (error) => {
        console.error('Error rejecting proposal:', error);
        this.toastService.error('Erro ao rejeitar proposta');
      }
    });
  }
}

