import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PropertyProposal, CreateProposalRequest } from '../models/proposal.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProposalService {
  private readonly apiUrl = `${environment.apiUrl}/proposals`;

  constructor(private http: HttpClient) {}

  create(proposal: CreateProposalRequest): Observable<Result<string>> {
    return this.http.post<Result<string>>(this.apiUrl, proposal);
  }

  getById(proposalId: string): Observable<Result<PropertyProposal>> {
    return this.http.get<Result<PropertyProposal>>(`${this.apiUrl}/${proposalId}`);
  }

  getByClient(clientId: string): Observable<Result<PropertyProposal[]>> {
    return this.http.get<Result<PropertyProposal[]>>(`${this.apiUrl}/client/${clientId}`);
  }

  getByAgent(agentId: string): Observable<Result<PropertyProposal[]>> {
    return this.http.get<Result<PropertyProposal[]>>(`${this.apiUrl}/agent/${agentId}`);
  }

  approve(proposalId: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${proposalId}/approve`, {});
  }

  reject(proposalId: string, reason: string): Observable<Result<boolean>> {
    return this.http.put<Result<boolean>>(`${this.apiUrl}/${proposalId}/reject`, { proposalId, rejectionReason: reason });
  }

  addNegotiation(proposalId: string, senderId: string, message: string, counterOffer?: number): Observable<Result<string>> {
    return this.http.post<Result<string>>(`${this.apiUrl}/${proposalId}/negotiate`, {
      proposalId,
      senderId,
      message,
      counterOffer
    });
  }
}

