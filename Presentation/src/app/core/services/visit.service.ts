import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
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
    console.log('VisitService.scheduleVisit() called with:', request);
    // Converter timeSlot de number para o formato esperado
    const body = {
      propertyId: request.propertyId,
      clientId: request.clientId,
      realEstateAgentId: request.realEstateAgentId,
      visitDate: request.visitDate,
      timeSlot: request.timeSlot, // Já é number (0-4)
      notes: request.notes
    };
    console.log('Sending POST request to:', `${this.apiUrl}`, 'with body:', body);
    return this.http.post(`${this.apiUrl}`, body);
  }

  // Get visits by client
  getVisitsByClient(clientId: string): Observable<PropertyVisit[]> {
    return this.http.get<{ visits: PropertyVisit[], totalCount: number }>(`${this.apiUrl}/client/${clientId}`).pipe(
      map(response => {
        const visits = response.visits || [];
        // Mapear agentId (do backend) para realEstateAgentId (do frontend)
        return visits.map(visit => {
          const visitAny = visit as any;
          return {
            ...visit,
            realEstateAgentId: visitAny.agentId || visit.realEstateAgentId || ''
          };
        });
      })
    );
  }

  // Get visits by agent
  getVisitsByAgent(agentId: string): Observable<PropertyVisit[]> {
    return this.http.get<{ visits: PropertyVisit[], totalCount: number }>(`${this.apiUrl}/agent/${agentId}`).pipe(
      map(response => {
        const visits = response.visits || [];
        // Mapear agentId (do backend) para realEstateAgentId (do frontend)
        return visits.map(visit => {
          const visitAny = visit as any;
          return {
            ...visit,
            realEstateAgentId: visitAny.agentId || visit.realEstateAgentId || ''
          };
        });
      })
    );
  }

  // Get available time slots
  getAvailableTimeSlots(propertyId: string, date: string): Observable<{ availableSlots: string[] }> {
    const params = new HttpParams()
      .set('propertyId', propertyId)
      .set('visitDate', date);
    
    return this.http.get<{ availableSlots: string[] }>(`${this.apiUrl}/available-slots`, { params });
  }

  // Confirm a visit
  confirmVisit(request: ConfirmVisitRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${request.visitId}/confirm`, {});
  }

  // Cancel a visit
  cancelVisit(request: CancelVisitRequest): Observable<any> {
    return this.http.put(`${this.apiUrl}/${request.visitId}/cancel`, { cancellationReason: request.reason });
  }

  // Get visit by ID
  getVisitById(visitId: string): Observable<PropertyVisit> {
    return this.http.get<PropertyVisit>(`${this.apiUrl}/${visitId}`);
  }
}

