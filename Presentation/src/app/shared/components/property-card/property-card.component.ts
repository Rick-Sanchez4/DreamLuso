import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Property } from '../../../core/models/property.model';
import { RatingStarsComponent } from '../rating-stars/rating-stars.component';

@Component({
  selector: 'app-property-card',
  standalone: true,
  imports: [CommonModule, RatingStarsComponent],
  template: `
    <div 
      (click)="navigateToDetail()"
      class="group relative overflow-hidden rounded-2xl bg-white shadow-md hover:shadow-2xl transition-all duration-300 cursor-pointer transform hover:-translate-y-2">
      
      <!-- Image -->
      <div class="relative h-56 overflow-hidden">
        <img 
          [src]="property.images[0]?.url || 'https://via.placeholder.com/400x300?text=Sem+Imagem'"
          [alt]="property.title"
          class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500">
        
        <!-- Status Badge -->
        <div class="absolute top-3 left-3">
          <span [class]="getStatusBadgeClass()">
            {{ property.status }}
          </span>
        </div>

        <!-- Transaction Type Badge -->
        <div class="absolute top-3 right-3">
          <span class="px-3 py-1 bg-white/90 backdrop-blur-sm rounded-full text-xs font-semibold text-gray-700">
            {{ property.transactionType === 'Sale' ? 'Venda' : 'Arrendamento' }}
          </span>
        </div>
      </div>

      <!-- Content -->
      <div class="p-5">
        <!-- Title -->
        <h3 class="text-lg font-semibold text-gray-900 mb-2 line-clamp-2 group-hover:text-primary-600 transition-colors">
          {{ property.title }}
        </h3>

        <!-- Location -->
        <div class="flex items-center gap-2 text-gray-600 mb-3">
          <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
          </svg>
          <span class="text-sm">{{ property.address.city }}, {{ property.address.district }}</span>
        </div>

        <!-- Features -->
        <div class="flex items-center gap-4 text-sm text-gray-600 mb-4">
          <div class="flex items-center gap-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
            </svg>
            <span>{{ property.bedrooms }} quartos</span>
          </div>
          <div class="flex items-center gap-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
            </svg>
            <span>{{ property.bathrooms }} WC</span>
          </div>
          <div class="flex items-center gap-1">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 8V4m0 0h4M4 4l5 5m11-1V4m0 0h-4m4 0l-5 5M4 16v4m0 0h4m-4 0l5-5m11 5l-5-5m5 5v-4m0 4h-4"/>
            </svg>
            <span>{{ property.area }}m²</span>
          </div>
        </div>

        <!-- Rating -->
        @if (property.averageRating) {
          <div class="mb-4">
            <app-rating-stars 
              [rating]="property.averageRating"
              [totalReviews]="property.totalComments"
              [showCount]="true"
              [interactive]="false">
            </app-rating-stars>
          </div>
        }

        <!-- Price -->
        <div class="flex items-center justify-between pt-4 border-t border-gray-200">
          <div>
            <p class="text-2xl font-bold text-primary-600">
              €{{ property.price.toLocaleString('pt-PT') }}
            </p>
            @if (property.transactionType === 'Rent') {
              <p class="text-xs text-gray-500">por mês</p>
            }
          </div>
          <button class="px-4 py-2 bg-primary-500 hover:bg-primary-600 text-white rounded-lg font-medium transition-colors">
            Ver Detalhes
          </button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .line-clamp-2 {
      display: -webkit-box;
      -webkit-line-clamp: 2;
      -webkit-box-orient: vertical;
      overflow: hidden;
    }
  `]
})
export class PropertyCardComponent {
  @Input() property!: Property;

  constructor(private router: Router) {}

  navigateToDetail(): void {
    this.router.navigate(['/property', this.property.id]);
  }

  getStatusBadgeClass(): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold backdrop-blur-sm';
    switch (this.property.status) {
      case 'Available':
        return `${baseClass} bg-green-500/90 text-white`;
      case 'Reserved':
        return `${baseClass} bg-yellow-500/90 text-white`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-500/90 text-white`;
      default:
        return `${baseClass} bg-gray-500/90 text-white`;
    }
  }
}

