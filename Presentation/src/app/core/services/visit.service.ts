import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  PropertyVisit, 
  ScheduleVisitRequest, 
  ConfirmVisitRequest, 
  CancelVisitRequest,
  AvailableTimeSlot 
} from '../models/visit.model';

@Injectable({
  providedIn: 'root'
})
export class VisitService {
  private readonly apiUrl = `${environment.apiUrl}/visits`;

  constructor(private http: HttpClient) {}

  // Schedule a visit
  scheduleVisit(request: ScheduleVisitRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}`, request);
  }

  // Get visits by client
  getVisitsByClient(clientId: string): Observable<PropertyVisit[]> {
    return this.http.get<PropertyVisit[]>(`${this.apiUrl}/client/${clientId}`);
  }

  // Get visits by agent
  getVisitsByAgent(agentId: string): Observable<PropertyVisit[]> {
    return this.http.get<PropertyVisit[]>(`${this.apiUrl}/agent/${agentId}`);
  }

  // Get available time slots
  getAvailableTimeSlots(propertyId: string, agentId: string, date: string): Observable<AvailableTimeSlot[]> {
    const params = new HttpParams()
      .set('propertyId', propertyId)
      .set('agentId', agentId)
      .set('date', date);
    
    return this.http.get<AvailableTimeSlot[]>(`${this.apiUrl}/available-slots`, { params });
  }

  // Confirm a visit
  confirmVisit(request: ConfirmVisitRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/confirm`, request);
  }

  // Cancel a visit
  cancelVisit(request: CancelVisitRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/cancel`, request);
  }

  // Get visit by ID
  getVisitById(visitId: string): Observable<PropertyVisit> {
    return this.http.get<PropertyVisit>(`${this.apiUrl}/${visitId}`);
  }
}

