import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PropertyService } from '../../../../core/services/property.service';
import { Property, PropertySearchFilters, PropertyType, TransactionType } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NavbarComponent, FooterComponent, PropertyCardComponent],
  template: `
    <div class="min-h-screen bg-gray-50">
      <app-navbar></app-navbar>

      <div class="pt-20 pb-12">
        <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          
          <!-- Header -->
          <div class="mb-8">
            <h1 class="text-4xl font-bold text-gray-900 mb-2">Descubra o Seu Imóvel Ideal</h1>
            <p class="text-gray-600">{{ totalProperties }} imóveis disponíveis</p>
          </div>

          <!-- Filters -->
          <div class="bg-white rounded-xl shadow-md p-6 mb-8">
            <form (ngSubmit)="applyFilters()" class="grid grid-cols-1 md:grid-cols-4 gap-4">
              
              <!-- City -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Cidade</label>
                <input
                  type="text"
                  [(ngModel)]="filters.city"
                  name="city"
                  placeholder="Ex: Lisboa, Porto..."
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
              </div>

              <!-- Price Range -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Preço Mín</label>
                <input
                  type="number"
                  [(ngModel)]="filters.minPrice"
                  name="minPrice"
                  placeholder="€0"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
              </div>

              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Preço Máx</label>
                <input
                  type="number"
                  [(ngModel)]="filters.maxPrice"
                  name="maxPrice"
                  placeholder="€1.000.000"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
              </div>

              <!-- Property Type -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Tipo</label>
                <select
                  [(ngModel)]="filters.propertyType"
                  name="propertyType"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
                  <option [ngValue]="undefined">Todos</option>
                  <option value="Apartment">Apartamento</option>
                  <option value="House">Casa</option>
                  <option value="Villa">Moradia</option>
                  <option value="Land">Terreno</option>
                  <option value="Commercial">Comercial</option>
                </select>
              </div>

              <!-- Bedrooms -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Quartos</label>
                <select
                  [(ngModel)]="filters.minBedrooms"
                  name="minBedrooms"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
                  <option [ngValue]="undefined">Qualquer</option>
                  <option [ngValue]="1">1+</option>
                  <option [ngValue]="2">2+</option>
                  <option [ngValue]="3">3+</option>
                  <option [ngValue]="4">4+</option>
                </select>
              </div>

              <!-- Transaction Type -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Negócio</label>
                <select
                  [(ngModel)]="filters.transactionType"
                  name="transactionType"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
                  <option [ngValue]="undefined">Todos</option>
                  <option value="Sale">Venda</option>
                  <option value="Rent">Arrendamento</option>
                </select>
              </div>

              <!-- Bathrooms -->
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-2">Casas de Banho</label>
                <select
                  [(ngModel)]="filters.minBathrooms"
                  name="minBathrooms"
                  class="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-primary-500 focus:border-transparent">
                  <option [ngValue]="undefined">Qualquer</option>
                  <option [ngValue]="1">1+</option>
                  <option [ngValue]="2">2+</option>
                  <option [ngValue]="3">3+</option>
                </select>
              </div>

              <!-- Submit -->
              <div class="flex items-end">
                <button
                  type="submit"
                  class="w-full px-6 py-2 bg-primary-500 hover:bg-primary-600 text-white font-medium rounded-lg transition-all">
                  Aplicar Filtros
                </button>
              </div>
            </form>
          </div>

          <!-- Properties Grid -->
          @if (loading) {
            <div class="flex justify-center items-center h-64">
              <div class="animate-spin rounded-full h-16 w-16 border-4 border-primary-500 border-t-transparent"></div>
            </div>
          } @else if (properties.length === 0) {
            <div class="text-center py-20">
              <svg class="w-24 h-24 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
              </svg>
              <h3 class="text-xl font-semibold text-gray-900 mb-2">Nenhum imóvel encontrado</h3>
              <p class="text-gray-600 mb-6">Tente ajustar os filtros de pesquisa</p>
              <button (click)="clearFilters()" class="px-6 py-3 bg-primary-500 hover:bg-primary-600 text-white font-medium rounded-lg transition-all">
                Limpar Filtros
              </button>
            </div>
          } @else {
            <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              @for (property of properties; track property.id) {
                <app-property-card [property]="property"></app-property-card>
              }
            </div>
          }
        </div>
      </div>

      <app-footer></app-footer>
    </div>
  `
})
export class PropertiesComponent implements OnInit {
  properties: Property[] = [];
  totalProperties: number = 0;
  loading: boolean = false;
  filters: PropertySearchFilters = {};

  constructor(
    private propertyService: PropertyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    // Get filters from query params
    this.route.queryParams.subscribe(params => {
      if (params['city']) this.filters.city = params['city'];
      if (params['transactionType']) this.filters.transactionType = params['transactionType'];
      if (params['propertyType']) this.filters.propertyType = params['propertyType'];
      
      this.loadProperties();
    });
  }

  loadProperties(): void {
    this.loading = true;
    
    if (Object.keys(this.filters).length > 0) {
      this.propertyService.search(this.filters).subscribe(result => {
        if (result.isSuccess && result.value) {
          this.properties = result.value;
          this.totalProperties = result.value.length;
        }
        this.loading = false;
      });
    } else {
      this.propertyService.getAll().subscribe(result => {
        if (result.isSuccess && result.value) {
          this.properties = result.value;
          this.totalProperties = result.value.length;
        }
        this.loading = false;
      });
    }
  }

  applyFilters(): void {
    this.loadProperties();
  }

  clearFilters(): void {
    this.filters = {};
    this.loadProperties();
  }
}

