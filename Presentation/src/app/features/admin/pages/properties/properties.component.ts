import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property } from '../../../../core/models/property.model';

@Component({
  selector: 'app-admin-properties',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
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
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadProperties();
  }

  loadProperties(): void {
    this.loading = true;
    this.propertyService.getAll().subscribe(result => {
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
    if (!confirm(`Tem certeza que deseja eliminar "${propertyTitle}"? Esta ação é irreversível!`)) {
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

  getAvailableCount(): number {
    return this.filteredProperties.filter(p => p.status === 'Available').length;
  }

  getReservedCount(): number {
    return this.filteredProperties.filter(p => p.status === 'Reserved').length;
  }

  getSoldRentedCount(): number {
    return this.filteredProperties.filter(p => p.status === 'Sold' || p.status === 'Rented').length;
  }
}

