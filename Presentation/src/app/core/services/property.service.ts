import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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
        if (result.value) {
          result.value = result.value.map((prop: any) => this.transformPropertyImages(prop));
        }
        return result;
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

