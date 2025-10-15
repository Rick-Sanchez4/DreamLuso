import { Component, OnInit } from '@angular/core';
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
export class LandingComponent implements OnInit {
  searchCity: string = '';
  featuredProperties: Property[] = [];
  loading: boolean = false;

  constructor(
    private propertyService: PropertyService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFeaturedProperties();
  }

  loadFeaturedProperties(): void {
    this.loading = true;
    this.propertyService.getAll().subscribe(result => {
      if (result.isSuccess && result.value) {
        this.featuredProperties = result.value.slice(0, 6); // Top 6
      }
      this.loading = false;
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

