import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PropertyService } from '../../../../core/services/property.service';
import { Property, PropertySearchFilters, PropertyType, TransactionType } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { PaginationComponent, PaginationInfo } from '../../../../shared/components/pagination/pagination.component';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-properties',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, NavbarComponent, FooterComponent, PropertyCardComponent, PaginationComponent],
  templateUrl: './properties.component.html',
  styleUrl: './properties.component.scss'
})
export class PropertiesComponent implements OnInit {
  properties: Property[] = [];
  totalProperties: number = 0;
  loading: boolean = false;
  filters: PropertySearchFilters = {};
  
  // Pagination
  paginationInfo: PaginationInfo = {
    pageNumber: 1,
    pageSize: 12,
    totalCount: 0,
    totalPages: 0
  };

  constructor(
    private propertyService: PropertyService,
    private route: ActivatedRoute,
    private http: HttpClient
  ) {}

  ngOnInit(): void {
    // Get filters and pagination from query params
    this.route.queryParams.subscribe((params: any) => {
      if (params['city']) this.filters.city = params['city'];
      if (params['transactionType']) this.filters.transactionType = params['transactionType'];
      if (params['propertyType']) this.filters.propertyType = params['propertyType'];
      if (params['page']) this.paginationInfo.pageNumber = parseInt(params['page'], 10) || 1;
      if (params['pageSize']) this.paginationInfo.pageSize = parseInt(params['pageSize'], 10) || 12;
      
      this.loadProperties();
    });
  }

  loadProperties(): void {
    this.loading = true;
    
    // Build query string from filters and pagination
    let queryParams = `pageNumber=${this.paginationInfo.pageNumber}&pageSize=${this.paginationInfo.pageSize}`;
    if (this.filters.city) queryParams += `&municipality=${this.filters.city}`;
    if (this.filters.minPrice) queryParams += `&minPrice=${this.filters.minPrice}`;
    if (this.filters.maxPrice) queryParams += `&maxPrice=${this.filters.maxPrice}`;
    if (this.filters.propertyType) queryParams += `&type=${this.filters.propertyType}`;
    if (this.filters.minBedrooms) queryParams += `&minBedrooms=${this.filters.minBedrooms}`;
    if (this.filters.minBathrooms) queryParams += `&minBathrooms=${this.filters.minBathrooms}`;
    if (this.filters.transactionType) {
      // Convert TransactionType enum to number: Sale = 0, Rent = 1
      const transactionTypeNum = this.filters.transactionType === TransactionType.Sale ? 0 : 
                                  this.filters.transactionType === TransactionType.Rent ? 1 : null;
      if (transactionTypeNum !== null) {
        queryParams += `&transactionType=${transactionTypeNum}`;
      }
    }
    
    this.http.get<any>(`${environment.apiUrl}/properties?${queryParams}`).subscribe({
      next: (result) => {
        if (result && result.properties) {
          // Transform properties to ensure address and images are properly set
          this.properties = result.properties.map((prop: any) => this.transformProperty(prop));
          this.totalProperties = result.totalCount || 0;
          this.paginationInfo.totalCount = result.totalCount || 0;
          this.paginationInfo.totalPages = result.totalPages || Math.ceil((result.totalCount || 0) / this.paginationInfo.pageSize);
        } else {
          this.properties = [];
          this.totalProperties = 0;
          this.paginationInfo.totalCount = 0;
          this.paginationInfo.totalPages = 0;
        }
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Error loading properties:', error);
        this.properties = [];
        this.totalProperties = 0;
        this.paginationInfo.totalCount = 0;
        this.paginationInfo.totalPages = 0;
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.paginationInfo.pageNumber = 1; // Reset to first page when applying filters
    this.loadProperties();
  }

  clearFilters(): void {
    this.filters = {};
    this.paginationInfo.pageNumber = 1;
    this.loadProperties();
  }

  onPageChange(page: number): void {
    this.paginationInfo.pageNumber = page;
    this.loadProperties();
    // Scroll to top
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onPageSizeChange(pageSize: number): void {
    this.paginationInfo.pageSize = pageSize;
    this.paginationInfo.pageNumber = 1; // Reset to first page
    this.loadProperties();
  }

  private transformProperty(property: any): Property {
    // Transform imageUrls (array of filenames) into images (array of PropertyImage objects)
    if (property.imageUrls && Array.isArray(property.imageUrls)) {
      property.images = property.imageUrls.map((filename: string, index: number) => ({
        id: `${property.id}-${index}`,
        url: `http://localhost:5149/images/properties/${filename}`,
        isMain: index === 0
      }));
    } else if (!property.images) {
      property.images = [];
    }
    
    // Map Size to area (backend uses Size, frontend uses area)
    if (property.size !== undefined && property.area === undefined) {
      property.area = property.size;
    } else if (property.Size !== undefined && property.area === undefined) {
      property.area = property.Size;
    }
    
    // Map realEstateAgentId from different possible field names (backend returns AgentId)
    if (!property.realEstateAgentId) {
      if (property.agentId) {
        property.realEstateAgentId = property.agentId;
      } else if (property.AgentId) {
        property.realEstateAgentId = property.AgentId;
      } else if (property.realEstateAgent?.id) {
        property.realEstateAgentId = property.realEstateAgent.id;
      }
    }
    
    // Ensure address object exists if we have municipality/district
    if (!property.address && (property.municipality || property.district)) {
      property.address = {
        street: property.street || '',
        city: property.municipality || '',
        district: property.district || '',
        postalCode: property.postalCode || '',
        country: 'Portugal'
      };
    }
    
    return property as Property;
  }
}
