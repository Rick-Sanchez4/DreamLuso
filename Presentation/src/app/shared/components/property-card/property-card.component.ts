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
}

