import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property } from '../../../../core/models/property.model';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-agent-properties',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.scss'
})
export class AgentPropertiesComponent implements OnInit {
  currentUser: User | null = null;
  properties: Property[] = [];
  filteredProperties: Property[] = [];
  loading: boolean = true;
  
  // Filters
  statusFilter: string = 'all';
  typeFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private propertyService: PropertyService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadProperties();
    }
  }

  loadProperties(): void {
    this.loading = true;
    this.propertyService.getByAgent(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.properties = result.value;
        this.applyFilters();
      } else {
        this.toastService.error('Erro ao carregar imóveis');
      }
      this.loading = false;
    });
  }

  applyFilters(): void {
    this.filteredProperties = this.properties.filter(p => {
      const matchesStatus = this.statusFilter === 'all' || p.status === this.statusFilter;
      const matchesType = this.typeFilter === 'all' || p.propertyType === this.typeFilter;
      const matchesSearch = !this.searchTerm || 
        p.title?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        p.address.city?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
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

    this.propertyService.delete(propertyId).subscribe(result => {
      if (result.isSuccess) {
        this.toastService.success('Imóvel eliminado com sucesso!');
        this.loadProperties();
      } else {
        this.toastService.error('Erro ao eliminar imóvel');
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Available':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Reserved':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-100 text-red-800`;
      case 'Inactive':
        return `${baseClass} bg-gray-100 text-gray-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getTransactionTypeLabel(type: string): string {
    return type === 'Sale' ? 'Venda' : 'Arrendamento';
  }
}

