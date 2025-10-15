import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PropertyService } from '../../../../core/services/property.service';
import { Property, PropertySearchFilters, PropertyType, TransactionType } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { NavbarComponent } from '../../../../shared/components/navbar/navbar.component';
import { FooterComponent } from '../../../../shared/components/footer/footer.component';

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
    private route: ActivatedRoute
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
    
    if (Object.keys(this.filters).length > 0) {
      this.propertyService.search(this.filters).subscribe(result => {
        if (result.isSuccess && result.value) {
          this.properties = result.value;
          this.totalProperties = result.value.length;
        }
        this.loading = false;
      });
    } else {
      this.propertyService.getAll().subscribe(result => {
        if (result.isSuccess && result.value) {
          this.properties = result.value;
          this.totalProperties = result.value.length;
        }
        this.loading = false;
      });
    }
  }

  applyFilters(): void {
    this.loadProperties();
  }

  clearFilters(): void {
    this.filters = {};
    this.loadProperties();
  }
}
