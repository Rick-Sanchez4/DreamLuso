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
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

@Component({
  selector: 'app-agent-properties',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent],
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
        this.loadProperties();
      },
      error: (error) => {
        console.error('Error loading agent profile:', error);
        this.toastService.error('Erro ao carregar perfil do agente');
        this.loading = false;
      }
    });
  }

  loadProperties(): void {
    if (!this.agentId) return;
    
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/properties?agentId=${this.agentId}&pageSize=100`).subscribe({
      next: (result) => {
        if (result && result.properties) {
          this.properties = result.properties;
          this.applyFilters();
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading properties:', error);
        this.toastService.error('Erro ao carregar imóveis');
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredProperties = this.properties.filter(p => {
      const matchesStatus = this.statusFilter === 'all' || String(p.status) === this.statusFilter;
      const matchesType = this.typeFilter === 'all' || String(p.propertyType) === this.typeFilter;
      const matchesSearch = !this.searchTerm || 
        p.title?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.municipality?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.district?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesStatus && matchesType && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
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
}

