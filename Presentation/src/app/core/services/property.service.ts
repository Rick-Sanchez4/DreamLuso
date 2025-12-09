import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Property, PropertySearchFilters } from '../models/property.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  private readonly apiUrl = `${environment.apiUrl}/properties`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Result<Property[]>> {
    return this.http.get<Result<Property[]>>(this.apiUrl).pipe(
      map(result => {
        if (result && result.value) {
          result.value = result.value.map((prop: any) => this.transformPropertyImages(prop));
        }
        return result || { isSuccess: true, value: [] } as Result<Property[]>;
      }),
      catchError(error => {
        // Handle connection errors gracefully
        if (error.status === 0) {
          console.warn('Backend não está disponível. Retornando lista vazia de propriedades.');
          return of({ isSuccess: true, value: [] } as Result<Property[]>);
        }
        return throwError(() => error);
      })
    );
  }

  getById(id: string): Observable<Property> {
    return this.http.get<any>(`${this.apiUrl}/${id}`).pipe(
      map(response => this.transformPropertyImages(response))
    );
  }

  private transformPropertyImages(property: any): Property {
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
    
    // Map agentName if available (backend returns AgentName)
    if (!property.agentName) {
      if (property.agentName) {
        // Already set
      } else if (property.AgentName) {
        property.agentName = property.AgentName;
      } else if (property.realEstateAgent?.user?.name) {
        property.agentName = `${property.realEstateAgent.user.name.firstName} ${property.realEstateAgent.user.name.lastName}`;
      } else if (property.realEstateAgent?.name) {
        property.agentName = property.realEstateAgent.name;
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

  search(filters: PropertySearchFilters): Observable<Result<Property[]>> {
    return this.http.post<Result<Property[]>>(`${this.apiUrl}/search`, filters).pipe(
      map(result => {
        if (result.value) {
          result.value = result.value.map((prop: any) => this.transformPropertyImages(prop));
        }
        return result;
      })
    );
  }

  create(property: Partial<Property>): Observable<Result<string>> {
    return this.http.post<Result<string>>(this.apiUrl, property);
  }

  update(id: string, property: Partial<Property>): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${id}`, property);
  }

  delete(id: string): Observable<Result<boolean>> {
    return this.http.delete<Result<boolean>>(`${this.apiUrl}/${id}`);
  }

  getByAgent(agentId: string): Observable<Result<Property[]>> {
    return this.http.get<Result<Property[]>>(`${this.apiUrl}/agent/${agentId}`).pipe(
      map(result => {
        if (result.value) {
          result.value = result.value.map((prop: any) => this.transformPropertyImages(prop));
        }
        return result;
      })
    );
  }

  getPropertyRating(propertyId: string): Observable<Result<any>> {
    return this.http.get<Result<any>>(`${environment.apiUrl}/comments/property/${propertyId}/rating`);
  }
}

