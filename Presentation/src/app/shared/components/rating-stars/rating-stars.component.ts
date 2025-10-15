import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-rating-stars',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './rating-stars.component.html',
  styleUrl: './rating-stars.component.scss'
})
export class RatingStarsComponent {
  @Input() rating: number = 0;
  @Input() totalReviews?: number;
  @Input() showCount: boolean = true;
  @Input() interactive: boolean = false;
  @Output() ratingChange = new EventEmitter<number>();

  stars = [1, 2, 3, 4, 5];

  getStarClass(position: number): string {
    const baseClass = 'w-5 h-5 transition-all duration-200';
    if (position <= Math.floor(this.rating)) {
      return `${baseClass} star-filled`;
    } else if (position === Math.ceil(this.rating) && this.rating % 1 !== 0) {
      return `${baseClass} star-half`;
    }
    return `${baseClass} star-empty`;
  }

  onStarClick(rating: number): void {
    if (this.interactive) {
      this.rating = rating;
      this.ratingChange.emit(rating);
    }
  }
}

