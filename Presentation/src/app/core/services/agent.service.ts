import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

export interface AgentDashboardStats {
  totalProperties: number;
  activeProperties: number;
  totalProposals: number;
  pendingProposals: number;
  scheduledVisits: number;
  completedVisits: number;
  totalRevenue: number;
  totalCommissions: number;
  totalSales: number;
  averageRating: number;
}

export interface AgentProfile {
  id: string;
  userId: string;
  fullName: string;
  email: string;
  phone?: string;
  licenseNumber?: string;
  licenseExpiry?: string;
  officeEmail?: string;
  officePhone?: string;
  commissionRate?: number;
  totalSales: number;
  totalListings: number;
  totalRevenue: number;
  rating: number;
  reviewCount: number;
  isActive: boolean;
  specialization?: string;
  certifications?: string[];
  languagesSpoken?: string[];
  createdAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class AgentService {
  private apiUrl = `${environment.apiUrl}/agents`;
  private dashboardUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  // Get agent profile by user ID
  getByUserId(userId: string): Observable<AgentProfile> {
    return this.http.get<AgentProfile>(`${this.apiUrl}/user/${userId}`);
  }

  // Get agent profile by ID
  getById(agentId: string): Observable<AgentProfile> {
    return this.http.get<AgentProfile>(`${this.apiUrl}/${agentId}`);
  }

  // Get agent dashboard stats
  getDashboardStats(agentId: string): Observable<AgentDashboardStats> {
    return this.http.get<AgentDashboardStats>(`${this.dashboardUrl}/agent/${agentId}`);
  }

  // Update agent profile
  updateProfile(agentId: string, data: Partial<AgentProfile>): Observable<any> {
    return this.http.put(`${this.apiUrl}/${agentId}`, data);
  }
}

