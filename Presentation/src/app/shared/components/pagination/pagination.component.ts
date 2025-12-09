import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface PaginationInfo {
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})
export class PaginationComponent {
  @Input() paginationInfo: PaginationInfo = {
    pageNumber: 1,
    pageSize: 10,
    totalCount: 0,
    totalPages: 0
  };

  @Input() showPageSizeSelector: boolean = true;
  @Input() pageSizeOptions: number[] = [10, 20, 50, 100];

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  get pages(): number[] {
    const total = this.paginationInfo.totalPages;
    const current = this.paginationInfo.pageNumber;
    const pages: number[] = [];

    if (total <= 7) {
      // Show all pages if 7 or fewer
      for (let i = 1; i <= total; i++) {
        pages.push(i);
      }
    } else {
      // Always show first page
      pages.push(1);

      if (current > 3) {
        pages.push(-1); // Ellipsis
      }

      // Show pages around current
      const start = Math.max(2, current - 1);
      const end = Math.min(total - 1, current + 1);

      for (let i = start; i <= end; i++) {
        pages.push(i);
      }

      if (current < total - 2) {
        pages.push(-1); // Ellipsis
      }

      // Always show last page
      pages.push(total);
    }

    return pages;
  }

  get hasPrevious(): boolean {
    return this.paginationInfo.pageNumber > 1;
  }

  get hasNext(): boolean {
    return this.paginationInfo.pageNumber < this.paginationInfo.totalPages;
  }

  get startItem(): number {
    if (this.paginationInfo.totalCount === 0) return 0;
    return (this.paginationInfo.pageNumber - 1) * this.paginationInfo.pageSize + 1;
  }

  get endItem(): number {
    return Math.min(
      this.paginationInfo.pageNumber * this.paginationInfo.pageSize,
      this.paginationInfo.totalCount
    );
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.paginationInfo.totalPages && page !== this.paginationInfo.pageNumber) {
      this.pageChange.emit(page);
    }
  }

  previousPage(): void {
    if (this.hasPrevious) {
      this.goToPage(this.paginationInfo.pageNumber - 1);
    }
  }

  nextPage(): void {
    if (this.hasNext) {
      this.goToPage(this.paginationInfo.pageNumber + 1);
    }
  }

  onPageSizeChange(event: Event): void {
    const select = event.target as HTMLSelectElement;
    const newPageSize = parseInt(select.value, 10);
    this.pageSizeChange.emit(newPageSize);
  }

  isEllipsis(page: number): boolean {
    return page === -1;
  }
}

