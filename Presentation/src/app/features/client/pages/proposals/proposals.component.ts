import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProposalService } from '../../../../core/services/proposal.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ClientService } from '../../../../core/services/client.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { User } from '../../../../core/models/user.model';
import { PaginationComponent, PaginationInfo } from '../../../../shared/components/pagination/pagination.component';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';

@Component({
  selector: 'app-client-proposals',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ClientSidebarComponent, PaginationComponent],
  templateUrl: './proposals.component.html',
  styleUrl: './proposals.component.scss'
})
export class ClientProposalsComponent implements OnInit {
  currentUser: User | null = null;
  clientId: string | null = null;
  proposals: PropertyProposal[] = [];
  filteredProposals: PropertyProposal[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  typeFilter: string = 'all';
  searchTerm: string = '';

  // Pagination (client-side for now)
  paginationInfo: PaginationInfo = {
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  };

  constructor(
    private proposalService: ProposalService,
    private authService: AuthService,
    private clientService: ClientService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadClientProfile();
    }
  }

  loadClientProfile(): void {
    this.clientService.getByUserId(this.currentUser!.id).subscribe({
      next: (client: any) => {
        this.clientId = client?.id || null;
        this.loadProposals();
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  loadProposals(): void {
    if (!this.clientId) {
      this.loading = false;
      return;
    }
    
    this.loading = true;
    this.proposalService.getByClient(this.clientId).subscribe({
      next: (result) => {
        if (result.isSuccess && result.value) {
          this.proposals = result.value;
          this.applyFilters();
        } else {
          this.proposals = [];
          this.filteredProposals = [];
          this.paginationInfo.totalCount = 0;
          this.paginationInfo.totalPages = 0;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading proposals:', error);
        this.proposals = [];
        this.filteredProposals = [];
        this.paginationInfo.totalCount = 0;
        this.paginationInfo.totalPages = 0;
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    const filtered = this.proposals.filter(p => {
      const matchesStatus = this.statusFilter === 'all' || p.status === this.statusFilter;
      const matchesType = this.typeFilter === 'all' || p.type === this.typeFilter;
      const matchesSearch = !this.searchTerm || 
        p.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.proposalNumber.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesType && matchesSearch;
    });

    // Update pagination info
    this.paginationInfo.totalCount = filtered.length;
    this.paginationInfo.totalPages = Math.ceil(filtered.length / this.paginationInfo.pageSize);

    // Apply pagination
    const startIndex = (this.paginationInfo.pageNumber - 1) * this.paginationInfo.pageSize;
    const endIndex = startIndex + this.paginationInfo.pageSize;
    this.filteredProposals = filtered.slice(startIndex, endIndex);
  }

  onFilterChange(): void {
    this.paginationInfo.pageNumber = 1; // Reset to first page
    this.applyFilters();
  }

  onPageChange(page: number): void {
    this.paginationInfo.pageNumber = page;
    this.applyFilters();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onPageSizeChange(pageSize: number): void {
    this.paginationInfo.pageSize = pageSize;
    this.paginationInfo.pageNumber = 1;
    this.applyFilters();
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

