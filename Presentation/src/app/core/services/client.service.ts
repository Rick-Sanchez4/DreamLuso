import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface ClientDashboardStats {
  totalProposals: number;
  pendingProposals: number;
  approvedProposals: number;
  rejectedProposals: number;
  totalVisits: number;
  scheduledVisits: number;
  completedVisits: number;
  totalFavorites: number;
  totalContracts: number;
  activeContracts: number;
}

export interface ClientProfile {
  id: string;
  userId: string;
  fullName: string;
  email: string;
  phone?: string;
  nif?: string;
  citizenCard?: string;
  type: string;
  minBudget?: number;
  maxBudget?: number;
  preferredContactMethod?: string;
  isActive: boolean;
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class ClientService {
  private apiUrl = `${environment.apiUrl}/clients`;
  private dashboardUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  // Get client profile by user ID
  getByUserId(userId: string): Observable<ClientProfile> {
    return this.http.get<ClientProfile>(`${this.apiUrl}/user/${userId}`);
  }

  // Get client profile by ID
  getById(clientId: string): Observable<ClientProfile> {
    return this.http.get<ClientProfile>(`${this.apiUrl}/${clientId}`);
  }

  // Get client dashboard stats
  getDashboardStats(clientId: string): Observable<ClientDashboardStats> {
    return this.http.get<ClientDashboardStats>(`${this.dashboardUrl}/client/${clientId}`);
  }

  // Update client profile
  updateProfile(clientId: string, data: Partial<ClientProfile>): Observable<any> {
    return this.http.put(`${this.apiUrl}/${clientId}`, data);
  }
}

