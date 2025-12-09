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
  // Additional details
  agentName?: string;
  securityDeposit?: number;
  commission?: number;
  paymentFrequency?: string;
  autoRenewal?: boolean;
  signatureDate?: Date;
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
  
  // Modal
  showDetailsModal: boolean = false;
  selectedContract: Contract | null = null;
  loadingDetails: boolean = false;

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
    // Use query parameter instead of route parameter
    this.http.get<any>(`${environment.apiUrl}/contracts?agentId=${this.agentId}`).subscribe({
      next: (response) => {
        // Backend returns { contracts: [...], totalCount: ... }
        const contracts = response?.contracts || (Array.isArray(response) ? response : []);
        if (contracts && Array.isArray(contracts)) {
          // Map backend response to frontend Contract interface
          this.contracts = contracts.map((c: any) => {
            // Ensure totalValue is always a number
            const totalValue = typeof c.value === 'number' ? c.value : 
                              (typeof c.totalValue === 'number' ? c.totalValue : 0);
            
            // Ensure monthlyValue is a number or undefined
            const monthlyValue = typeof c.monthlyRent === 'number' ? c.monthlyRent : 
                                (typeof c.monthlyValue === 'number' ? c.monthlyValue : undefined);
            
            return {
              id: c.id,
              contractNumber: c.contractNumber || `CT-${c.id.substring(0, 8).toUpperCase()}`,
              propertyId: c.propertyId,
              propertyTitle: c.propertyTitle || 'Imóvel não encontrado',
              clientId: c.clientId,
              clientName: c.clientName || 'Cliente não encontrado',
              contractType: c.type || c.contractType || 'Sale',
              startDate: c.startDate ? new Date(c.startDate) : new Date(),
              endDate: c.endDate ? new Date(c.endDate) : undefined,
              monthlyValue: monthlyValue,
              totalValue: totalValue,
              status: c.status || 'Draft',
              createdAt: c.createdAt ? new Date(c.createdAt) : new Date(),
              // Additional fields
              agentName: c.agentName,
              securityDeposit: c.securityDeposit,
              commission: c.commission,
              paymentFrequency: c.paymentFrequency,
              autoRenewal: c.autoRenewal,
              signatureDate: c.signatureDate ? new Date(c.signatureDate) : undefined
            };
          });
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

  openDetailsModal(contract: Contract): void {
    this.selectedContract = contract;
    this.loadingDetails = true;
    this.showDetailsModal = true;
    
    // Load full contract details from API
    this.http.get<any>(`${environment.apiUrl}/contracts/${contract.id}`).subscribe({
      next: (response) => {
        // Update selected contract with full details
        if (response) {
          this.selectedContract = {
            ...contract,
            agentName: response.agentName || contract.agentName,
            securityDeposit: response.securityDeposit,
            commission: response.commission,
            paymentFrequency: response.paymentFrequency,
            autoRenewal: response.autoRenewal,
            signatureDate: response.signatureDate ? new Date(response.signatureDate) : undefined
          };
        }
        this.loadingDetails = false;
      },
      error: (error) => {
        console.error('Error loading contract details:', error);
        this.toastService.error('Erro ao carregar detalhes do contrato');
        this.loadingDetails = false;
      }
    });
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedContract = null;
    this.loadingDetails = false;
  }

  getPaymentFrequencyLabel(frequency?: string): string {
    if (!frequency) return 'N/A';
    switch (frequency.toLowerCase()) {
      case 'monthly':
        return 'Mensal';
      case 'quarterly':
        return 'Trimestral';
      case 'yearly':
        return 'Anual';
      default:
        return frequency;
    }
  }
}

