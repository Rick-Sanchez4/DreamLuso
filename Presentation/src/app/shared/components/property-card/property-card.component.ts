import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Property } from '../../../core/models/property.model';
import { RatingStarsComponent } from '../rating-stars/rating-stars.component';

@Component({
  selector: 'app-property-card',
  standalone: true,
  imports: [CommonModule, RatingStarsComponent],
  templateUrl: './property-card.component.html',
  styleUrl: './property-card.component.scss'
})
export class PropertyCardComponent {
  @Input() property!: Property;

  constructor(private router: Router) {}

  navigateToDetail(): void {
    this.router.navigate(['/property', this.property.id]);
  }

  getStatusBadgeClass(): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold backdrop-blur-sm';
    const statusStr = String(this.property.status);
    switch (statusStr) {
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

  getLocationText(): string {
    if (!this.property) {
      return 'Localização não disponível';
    }
    
    // Try direct properties first (municipality/district from API)
    if (this.property.municipality || this.property.district) {
      const municipality = this.property.municipality || '';
      const district = this.property.district || '';
      if (municipality || district) {
        return `${municipality}, ${district}`.replace(/^,\s*|,\s*$/g, '').trim() || 'Localização não disponível';
      }
    }
    
    // Fallback to address object
    if (this.property.address) {
      const city = this.property.address.city || '';
      const district = this.property.address.district || '';
      if (city || district) {
        return `${city}, ${district}`.replace(/^,\s*|,\s*$/g, '').trim() || 'Localização não disponível';
      }
    }
    
    return 'Localização não disponível';
  }

  getImageUrl(): string {
    if (!this.property) {
      return this.getPlaceholderUrl();
    }
    
    if (this.property.images && Array.isArray(this.property.images) && this.property.images.length > 0) {
      const firstImage = this.property.images[0];
      if (firstImage && firstImage.url && firstImage.url.trim() !== '') {
        return firstImage.url;
      }
    }
    
    return this.getPlaceholderUrl();
  }

  private getPlaceholderUrl(): string {
    // Use a data URI as fallback to avoid external requests
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48cmVjdCB3aWR0aD0iNDAwIiBoZWlnaHQ9IjMwMCIgZmlsbD0iI2YzZjRmNiIvPjx0ZXh0IHg9IjUwJSIgeT0iNTAlIiBmb250LWZhbWlseT0iQXJpYWwiIGZvbnQtc2l6ZT0iMTgiIGZpbGw9IiM5Y2EzYWYiIHRleHQtYW5jaG9yPSJtaWRkbGUiIGR5PSIuM2VtIj5TZW0gSW1hZ2VtPC90ZXh0Pjwvc3ZnPg==';
  }

  getAreaDisplay(): string {
    if (!this.property) {
      return '-';
    }
    
    // Check area property (already mapped from Size)
    if (this.property.area !== undefined && this.property.area !== null && this.property.area > 0) {
      return this.property.area.toString();
    }
    
    // Fallback: try to access any property that might have the value (for type safety, we check the actual object)
    const prop = this.property as any;
    if (prop.size !== undefined && prop.size !== null && prop.size > 0) {
      return prop.size.toString();
    }
    if (prop.Size !== undefined && prop.Size !== null && prop.Size > 0) {
      return prop.Size.toString();
    }
    
    return '-';
  }
}

