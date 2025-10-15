import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
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
    return this.http.get<Result<Property[]>>(this.apiUrl);
  }

  getById(id: string): Observable<Result<Property>> {
    return this.http.get<Result<Property>>(`${this.apiUrl}/${id}`);
  }

  search(filters: PropertySearchFilters): Observable<Result<Property[]>> {
    return this.http.post<Result<Property[]>>(`${this.apiUrl}/search`, filters);
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
    return this.http.get<Result<Property[]>>(`${this.apiUrl}/agent/${agentId}`);
  }

  getPropertyRating(propertyId: string): Observable<Result<any>> {
    return this.http.get<Result<any>>(`${environment.apiUrl}/comments/property/${propertyId}/rating`);
  }
}

