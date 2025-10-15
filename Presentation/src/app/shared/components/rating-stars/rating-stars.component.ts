import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-rating-stars',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="flex items-center gap-1">
      @for (star of stars; track $index) {
        <svg
          (click)="onStarClick($index + 1)"
          [class]="getStarClass($index + 1)"
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="currentColor"
          class="w-5 h-5 transition-all duration-200"
          [class.cursor-pointer]="interactive"
          [class.hover:scale-110]="interactive">
          <path fill-rule="evenodd" d="M10.788 3.21c.448-1.077 1.976-1.077 2.424 0l2.082 5.007 5.404.433c1.164.093 1.636 1.545.749 2.305l-4.117 3.527 1.257 5.273c.271 1.136-.964 2.033-1.96 1.425L12 18.354 7.373 21.18c-.996.608-2.231-.29-1.96-1.425l1.257-5.273-4.117-3.527c-.887-.76-.415-2.212.749-2.305l5.404-.433 2.082-5.006z" clip-rule="evenodd" />
        </svg>
      }
      @if (showCount && totalReviews) {
        <span class="ml-2 text-sm text-gray-600">({{ totalReviews }})</span>
      }
    </div>
  `,
  styles: [`
    .star-filled {
      @apply text-yellow-400;
    }
    .star-half {
      @apply text-yellow-400;
    }
    .star-empty {
      @apply text-gray-300;
    }
  `]
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

