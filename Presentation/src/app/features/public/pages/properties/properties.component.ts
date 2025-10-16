import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PropertyService } from '../../../../core/services/property.service';
import { Property, PropertySearchFilters, PropertyType, TransactionType } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NavbarComponent, FooterComponent, PropertyCardComponent],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.scss'
})
export class PropertiesComponent implements OnInit {
  properties: Property[] = [];
  totalProperties: number = 0;
  loading: boolean = false;
  filters: PropertySearchFilters = {};

  constructor(
    private propertyService: PropertyService,
    private route: ActivatedRoute,
    private http: HttpClient
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
    
    // Build query string from filters
    let queryParams = 'pageSize=100';
    if (this.filters.city) queryParams += `&municipality=${this.filters.city}`;
    if (this.filters.minPrice) queryParams += `&minPrice=${this.filters.minPrice}`;
    if (this.filters.maxPrice) queryParams += `&maxPrice=${this.filters.maxPrice}`;
    if (this.filters.propertyType) queryParams += `&type=${this.filters.propertyType}`;
    if (this.filters.minBedrooms) queryParams += `&minBedrooms=${this.filters.minBedrooms}`;
    
    this.http.get<any>(`${environment.apiUrl}/properties?${queryParams}`).subscribe({
      next: (result) => {
        if (result && result.properties) {
          this.properties = result.properties;
          this.totalProperties = result.totalCount || result.properties.length;
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading properties:', error);
        this.properties = [];
        this.totalProperties = 0;
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.loadProperties();
  }

  clearFilters(): void {
    this.filters = {};
    this.loadProperties();
  }
}
