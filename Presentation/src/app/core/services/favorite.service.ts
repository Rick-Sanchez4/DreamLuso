import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { Property } from '../models/property.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  private readonly apiUrl = `${environment.apiUrl}/clients`;

  constructor(private http: HttpClient) {}

  /**
   * Get all favorite properties for a client
   */
  getFavorites(clientId: string): Observable<Result<Property[]>> {
    return this.http.get<any>(`${this.apiUrl}/${clientId}/favorites`).pipe(
      map((response: any) => {
        // Handle both Result format and direct response
        const properties = response?.properties || response?.value?.properties || [];
        const transformedProperties = properties.map((prop: any) => this.transformPropertyImages(prop));
        return { isSuccess: true, value: transformedProperties } as Result<Property[]>;
      })
    );
  }

  /**
   * Add a property to favorites
   */
  addFavorite(clientId: string, propertyId: string): Observable<Result<void>> {
    return this.http.post<any>(`${this.apiUrl}/${clientId}/favorites`, { propertyId }).pipe(
      map(() => ({ isSuccess: true } as Result<void>))
    );
  }

  /**
   * Remove a property from favorites
   */
  removeFavorite(clientId: string, propertyId: string): Observable<Result<void>> {
    return this.http.delete<any>(`${this.apiUrl}/${clientId}/favorites/${propertyId}`).pipe(
      map(() => ({ isSuccess: true } as Result<void>))
    );
  }

  /**
   * Check if a property is favorited (requires client to have favorites loaded)
   * This is a helper method - the actual check should be done server-side or cached
   */
  isFavorited(propertyId: string, favoritePropertyIds: string[]): boolean {
    return favoritePropertyIds.includes(propertyId);
  }

  /**
   * Transform property images from backend format to frontend format
   */
  private transformPropertyImages(property: any): Property {
    // Transform imageUrls (array of filenames) into images (array of PropertyImage objects)
    if (property.imageUrls && Array.isArray(property.imageUrls)) {
      property.images = property.imageUrls.map((filename: string, index: number) => ({
        id: `${property.id}-${index}`,
        url: `${environment.apiUrl.replace('/api', '')}/images/properties/${filename}`,
        isMain: index === 0
      }));
    } else if (!property.images) {
      property.images = [];
    }
    
    // Ensure address object exists
    if (!property.address && (property.street || property.municipality)) {
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

