import { Component, OnInit, OnDestroy, HostListener, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PropertyService } from '../../../../core/services/property.service';
import { Property, PropertySearchFilters } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';

@Component({
  selector: 'app-landing',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, NavbarComponent, FooterComponent, PropertyCardComponent],
  templateUrl: './landing.component.html',
  styleUrl: './landing.component.scss'
})
export class LandingComponent implements OnInit, OnDestroy {
  searchCity: string = '';
  featuredProperties: Property[] = [];
  loading: boolean = false;
  scrollY: number = 0;

  constructor(
    private propertyService: PropertyService,
    public router: Router
  ) {}

  @HostListener('window:scroll')
  onScroll(): void {
    this.scrollY = window.scrollY;
    this.handleScrollAnimations();
  }

  ngOnInit(): void {
    this.loadFeaturedProperties();
    this.initScrollAnimations();
  }

  ngOnDestroy(): void {
    // Cleanup if needed
  }

  private initScrollAnimations(): void {
    // Intersection Observer for fade-in animations
    const observerOptions = {
      threshold: 0.1,
      rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver((entries) => {
      entries.forEach(entry => {
        if (entry.isIntersecting) {
          entry.target.classList.add('animate-fade-in-up');
          entry.target.classList.remove('opacity-0');
        }
      });
    }, observerOptions);

    // Observe elements with data-animate attribute
    setTimeout(() => {
      const animateElements = document.querySelectorAll('[data-animate]');
      animateElements.forEach(el => {
        el.classList.add('opacity-0');
        observer.observe(el);
      });
    }, 100);
  }

  private handleScrollAnimations(): void {
    // Parallax effect for hero section elements
    const heroElements = document.querySelectorAll('.parallax-slow');
    heroElements.forEach((el: any) => {
      const speed = 0.5;
      el.style.transform = `translateY(${this.scrollY * speed}px)`;
    });
  }

  loadFeaturedProperties(): void {
    this.loading = true;
    this.propertyService.getAll().subscribe({
      next: (result: any) => {
        if (result.isSuccess && result.value) {
          this.featuredProperties = result.value.slice(0, 6); // Top 6
        } else {
          this.featuredProperties = [];
        }
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error loading featured properties:', error);
        this.featuredProperties = [];
        this.loading = false;
        // Silently handle connection errors - backend might not be running
        // No need to show error to user on landing page
      }
    });
  }

  onSearch(): void {
    const filters: PropertySearchFilters = {};
    if (this.searchCity) {
      filters.city = this.searchCity;
    }
    this.router.navigate(['/properties'], { queryParams: filters });
  }

  navigateToProperties(type?: string): void {
    if (type) {
      this.router.navigate(['/properties'], { queryParams: { transactionType: type } });
    } else {
      this.router.navigate(['/properties']);
    }
  }
}

