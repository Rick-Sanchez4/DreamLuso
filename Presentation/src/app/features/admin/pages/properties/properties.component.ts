import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property, PropertyStatus } from '../../../../core/models/property.model';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-admin-properties',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.scss'
})
export class AdminPropertiesComponent implements OnInit {
  properties: Property[] = [];
  filteredProperties: Property[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  typeFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private propertyService: PropertyService,
    private toastService: ToastService,
    private http: HttpClient,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadProperties();
  }

  loadProperties(): void {
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/properties?pageSize=100`).subscribe({
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
      const statusStr = String(p.status);
      const matchesStatus = this.statusFilter === 'all' || statusStr === this.statusFilter;
      const typeStr = String(p.propertyType);
      const matchesType = this.typeFilter === 'all' || typeStr === this.typeFilter;
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

  viewProperty(propertyId: string): void {
    // Navigate to property page (same tab)
    this.router.navigate(['/property', propertyId]);
  }

  isPropertyInactive(property: Property): boolean {
    const statusStr = String(property.status);
    return statusStr === 'Inactive' || 
           statusStr === 'Sold' || 
           statusStr === 'Rented' || 
           statusStr === 'Unavailable';
  }

  togglePropertyStatus(property: any): void {
    const isInactive = this.isPropertyInactive(property);
    const action = isInactive ? 'ativar' : 'desativar';
    if (!confirm(`Tem certeza que deseja ${action} "${property.title}"?`)) {
      return;
    }

    const newStatus = isInactive ? 'Available' : 'Unavailable';
    
    // Update property status
    this.http.put(`${environment.apiUrl}/properties/${property.id}`, {
      ...property,
      status: newStatus
    }).subscribe({
      next: () => {
        this.toastService.success(`Imóvel ${action === 'ativar' ? 'ativado' : 'desativado'} com sucesso!`);
        this.loadProperties();
      },
      error: (error) => {
        console.error('Error toggling property status:', error);
        this.toastService.error(`Erro ao ${action} imóvel`);
      }
    });
  }

  getStatusName(status: any): string {
    const statusStr = String(status);
    switch (statusStr) {
      case 'Available': return 'Disponível';
      case 'Reserved': return 'Reservado';
      case 'UnderContract': return 'Em Contrato';
      case 'Sold': return 'Vendido';
      case 'Rented': return 'Arrendado';
      case 'Unavailable': return 'Indisponível';
      case 'InNegotiation': return 'Em Negociação';
      case 'Inactive': return 'Inativo';
      default: return statusStr;
    }
  }

  getStatusBadgeClass(status: any): string {
    const statusStr = String(status);
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (statusStr) {
      case 'Available':
        return `${baseClass} bg-green-100 dark:bg-green-900/30 text-green-800 dark:text-green-400`;
      case 'Reserved':
        return `${baseClass} bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-400`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-100 dark:bg-red-900/30 text-red-800 dark:text-red-400`;
      case 'Unavailable':
      case 'Inactive':
        return `${baseClass} bg-gray-100 dark:bg-gray-900/30 text-gray-800 dark:text-gray-400`;
      case 'InNegotiation':
        return `${baseClass} bg-orange-100 dark:bg-orange-900/30 text-orange-800 dark:text-orange-400`;
      case 'UnderContract':
        return `${baseClass} bg-blue-100 dark:bg-blue-900/30 text-blue-800 dark:text-blue-400`;
      default:
        return `${baseClass} bg-gray-100 dark:bg-gray-900/30 text-gray-800 dark:text-gray-400`;
    }
  }

  getTransactionTypeLabel(type: string): string {
    return type === 'Sale' ? 'Venda' : 'Arrendamento';
  }

  getApprovedPropertiesCount(): number {
    return this.properties.filter(p => String(p.status) === 'Available').length;
  }

  getAvailableCount(): number {
    return this.filteredProperties.filter(p => p.status === PropertyStatus.Available).length;
  }

  getReservedCount(): number {
    return this.filteredProperties.filter(p => p.status === PropertyStatus.Reserved).length;
  }

  getSoldRentedCount(): number {
    return this.filteredProperties.filter(p => p.status === PropertyStatus.Sold || p.status === PropertyStatus.Rented).length;
  }
}

