import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { AgentService } from '../../../../core/services/agent.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { Property } from '../../../../core/models/property.model';
import { User } from '../../../../core/models/user.model';
import { PaginationComponent, PaginationInfo } from '../../../../shared/components/pagination/pagination.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

@Component({
  selector: 'app-agent-properties',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent, PaginationComponent],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.scss'
})
export class AgentPropertiesComponent implements OnInit {
  currentUser: User | null = null;
  agentId: string | null = null;
  properties: Property[] = [];
  filteredProperties: Property[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  typeFilter: string = 'all';
  searchTerm: string = '';

  // Pagination
  paginationInfo: PaginationInfo = {
    pageNumber: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 0
  };

  constructor(
    private propertyService: PropertyService,
    private agentService: AgentService,
    private authService: AuthService,
    private toastService: ToastService,
    private http: HttpClient,
    private router: Router,
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
        if (!agent.isActive) {
          this.toastService.warning('O seu perfil de agente está pendente de aprovação. Algumas funcionalidades podem estar limitadas.');
        }
        this.loadProperties();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        const errorMsg = error.error?.description || error.message || 'Erro desconhecido';
        if (error.status === 404) {
          this.toastService.error('Perfil de agente não encontrado. Por favor, contacte o suporte.');
        } else {
          this.toastService.error(`Erro ao carregar perfil do agente: ${errorMsg}`);
        }
        this.loading = false;
      }
    });
  }

  loadProperties(): void {
    if (!this.agentId) return;
    
    this.loading = true;
    let queryParams = `agentId=${this.agentId}&pageNumber=${this.paginationInfo.pageNumber}&pageSize=${this.paginationInfo.pageSize}`;
    
    // Add status filter if not 'all'
    if (this.statusFilter !== 'all') {
      // Map status string to number (backend expects PropertyStatus enum as int)
      // PropertyStatus enum: Available=0, Reserved=1, UnderContract=2, Sold=3, Rented=4, Unavailable=5, InNegotiation=6
      const statusMap: { [key: string]: number } = {
        'Available': 0,
        'Reserved': 1,
        'UnderContract': 2,
        'Sold': 3,
        'Rented': 4,
        'Unavailable': 5,
        'InNegotiation': 6,
        'Inactive': 5 // Map Inactive to Unavailable
      };
      if (statusMap[this.statusFilter] !== undefined) {
        queryParams += `&status=${statusMap[this.statusFilter]}`;
      }
    }
    
    // Add transaction type filter if not 'all'
    if (this.typeFilter !== 'all') {
      // Map transaction type string to number (backend expects TransactionType enum as int)
      const typeMap: { [key: string]: number } = {
        'Sale': 0,
        'Rent': 1
      };
      if (typeMap[this.typeFilter] !== undefined) {
        queryParams += `&transactionType=${typeMap[this.typeFilter]}`;
      }
    }
    
    // Add search term if provided
    if (this.searchTerm && this.searchTerm.trim()) {
      queryParams += `&searchTerm=${encodeURIComponent(this.searchTerm.trim())}`;
    }
    
    this.http.get<any>(`${environment.apiUrl}/properties?${queryParams}`).subscribe({
      next: (result) => {
        if (result && result.properties) {
          this.properties = result.properties;
          this.paginationInfo.totalCount = result.totalCount || 0;
          this.paginationInfo.totalPages = result.totalPages || Math.ceil((result.totalCount || 0) / this.paginationInfo.pageSize);
          // Apply client-side filters for additional filtering if needed
          this.applyFilters();
        } else {
          this.properties = [];
          this.filteredProperties = [];
          this.paginationInfo.totalCount = 0;
          this.paginationInfo.totalPages = 0;
        }
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error loading properties:', error);
        this.toastService.error('Erro ao carregar imóveis');
        this.properties = [];
        this.filteredProperties = [];
        this.paginationInfo.totalCount = 0;
        this.paginationInfo.totalPages = 0;
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    // Since filters are applied server-side, filteredProperties should match properties
    // But we can do additional client-side filtering if needed (e.g., for search term refinement)
    this.filteredProperties = this.properties.filter(p => {
      // Additional client-side search refinement (backend already filters, but we can refine)
      const matchesSearch = !this.searchTerm || 
        p.title?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.municipality?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.district?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesSearch;
    });
  }

  onFilterChange(): void {
    this.paginationInfo.pageNumber = 1; // Reset to first page when filters change
    this.loadProperties();
  }

  deleteProperty(propertyId: string, propertyTitle: string): void {
    if (!confirm(`Tem certeza que deseja eliminar "${propertyTitle}"?`)) {
      return;
    }

    this.propertyService.delete(propertyId).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Imóvel eliminado com sucesso!');
          this.loadProperties();
        } else {
          this.toastService.error('Erro ao eliminar imóvel');
        }
      },
      error: (error) => {
        console.error('Error deleting property:', error);
        this.toastService.error('Erro ao eliminar imóvel');
      }
    });
  }

  viewPropertyDetails(propertyId: string): void {
    // Navigate to property details page (public page)
    this.router.navigate(['/property', propertyId]);
  }

  viewPropertyOnSite(propertyId: string): void {
    // Open property in new tab
    window.open(`/property/${propertyId}`, '_blank');
  }

  editProperty(propertyId: string): void {
    // Navigate to edit page
    this.router.navigate(['/agent/properties/edit', propertyId]);
  }

  addProperty(): void {
    // Navigate to add new property page
    this.router.navigate(['/agent/properties/new']);
  }

  getStatusBadgeClass(status: any): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    const statusStr = String(status);
    switch (statusStr) {
      case 'Available':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/20 dark:text-green-400`;
      case 'Reserved':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/20 dark:text-yellow-400`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/20 dark:text-red-400`;
      case 'Inactive':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/20 dark:text-gray-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getTransactionTypeLabel(type: any): string {
    const typeStr = String(type);
    return typeStr === 'Sale' ? 'Venda' : 'Arrendamento';
  }

  onPageChange(pageNumber: number): void {
    this.paginationInfo.pageNumber = pageNumber;
    this.loadProperties();
  }

  onPageSizeChange(pageSize: number): void {
    this.paginationInfo.pageSize = pageSize;
    this.paginationInfo.pageNumber = 1; // Reset to first page
    this.loadProperties();
  }
}

