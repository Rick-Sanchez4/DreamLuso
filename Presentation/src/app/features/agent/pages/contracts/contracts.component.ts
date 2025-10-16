import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AgentService } from '../../../../core/services/agent.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { User } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

interface Contract {
  id: string;
  contractNumber: string;
  propertyId: string;
  propertyTitle: string;
  clientId: string;
  clientName: string;
  contractType: string;
  startDate: Date;
  endDate?: Date;
  monthlyValue?: number;
  totalValue: number;
  status: string;
  createdAt: Date;
}

@Component({
  selector: 'app-agent-contracts',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent],
  templateUrl: './contracts.component.html',
  styleUrl: './contracts.component.scss'
})
export class AgentContractsComponent implements OnInit {
  currentUser: User | null = null;
  agentId: string | null = null;
  contracts: Contract[] = [];
  filteredContracts: Contract[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  searchTerm: string = '';

  constructor(
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
        this.loadContracts();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        this.toastService.error('Erro ao carregar perfil do agente');
        this.loading = false;
      }
    });
  }

  loadContracts(): void {
    if (!this.agentId) return;
    
    this.loading = true;
    // For now using mock endpoint - will implement when backend is ready
    this.http.get<Contract[]>(`${environment.apiUrl}/contracts/agent/${this.agentId}`).subscribe({
      next: (contracts) => {
        if (contracts && Array.isArray(contracts)) {
          this.contracts = contracts;
          this.applyFilters();
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading contracts:', error);
        // Don't show error toast if it's 404 (no contracts yet)
        if (error.status !== 404) {
          this.toastService.error('Erro ao carregar contratos');
        }
        this.contracts = [];
        this.filteredContracts = [];
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredContracts = this.contracts.filter(c => {
      const matchesStatus = this.statusFilter === 'all' || c.status === this.statusFilter;
      const matchesSearch = !this.searchTerm || 
        c.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        c.clientName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        c.contractNumber.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesSearch;
    });

    // Sort by start date (most recent first)
    this.filteredContracts.sort((a, b) => 
      new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
    );
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Active':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400`;
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400`;
      case 'Expired':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400`;
      case 'Cancelled':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getContractTypeLabel(type: string): string {
    switch (type) {
      case 'Sale':
        return 'Compra e Venda';
      case 'Rent':
        return 'Arrendamento';
      default:
        return type;
    }
  }
}

