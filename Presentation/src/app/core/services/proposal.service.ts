import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { PropertyProposal, CreateProposalRequest } from '../models/proposal.model';
import { Result } from '../models/result.model';
import { environment } from '../../../environments/environment';

// Backend may return Guid as string or object
type GuidResponse = string | { id?: string };

@Injectable({
  providedIn: 'root'
})
export class ProposalService {
  private readonly apiUrl = `${environment.apiUrl}/proposals`;

  constructor(private http: HttpClient) {}

  create(proposal: CreateProposalRequest): Observable<Result<string>> {
    return this.http.post<GuidResponse>(this.apiUrl, proposal).pipe(
      map((response: GuidResponse) => {
        // Backend returns Guid directly (string or object), wrap it in Result format
        const id = typeof response === 'string' ? response : (response.id || String(response));
        return { isSuccess: true, value: id } as Result<string>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error creating proposal:', error);
        return of({
          isSuccess: false,
          error: {
            code: error.status === 400 ? 'BAD_REQUEST' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao criar proposta'
          }
        } as Result<string>);
      })
    );
  }

  getById(proposalId: string): Observable<Result<PropertyProposal>> {
    return this.http.get<PropertyProposal>(`${this.apiUrl}/${proposalId}`).pipe(
      map((proposal: PropertyProposal) => {
        // Backend returns PropertyProposal directly, wrap it in Result format
        return { isSuccess: true, value: proposal } as Result<PropertyProposal>;
      }),
      catchError((error: HttpErrorResponse) => {
        // Handle 404 or other errors
        console.error('Error loading proposal:', error);
        return of({ 
          isSuccess: false, 
          error: { 
            code: error.status === 404 ? 'NOT_FOUND' : 'ERROR',
            description: error.status === 404 ? 'Proposta não encontrada' : 'Erro ao carregar proposta'
          } 
        } as Result<PropertyProposal>);
      })
    );
  }

  getByClient(clientId: string): Observable<Result<PropertyProposal[]>> {
    return this.http.get<PropertyProposal[]>(`${this.apiUrl}/client/${clientId}`).pipe(
      map((proposals: PropertyProposal[]) => {
        // Backend returns array directly, wrap it in Result format
        return { isSuccess: true, value: proposals || [] } as Result<PropertyProposal[]>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error loading proposals by client:', error);
        // Handle 404 or connection errors gracefully
        if (error.status === 404 || error.status === 0) {
          return of({ isSuccess: true, value: [] } as Result<PropertyProposal[]>);
        }
        return of({
          isSuccess: false,
          error: {
            code: error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao carregar propostas'
          }
        } as Result<PropertyProposal[]>);
      })
    );
  }

  getByAgent(agentId: string): Observable<Result<PropertyProposal[]>> {
    return this.http.get<PropertyProposal[]>(`${this.apiUrl}/agent/${agentId}`).pipe(
      map((proposals: PropertyProposal[]) => {
        // Backend returns array directly, wrap it in Result format
        return { isSuccess: true, value: proposals || [] } as Result<PropertyProposal[]>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error loading proposals by agent:', error);
        // Handle 404 or connection errors gracefully
        if (error.status === 404 || error.status === 0) {
          return of({ isSuccess: true, value: [] } as Result<PropertyProposal[]>);
        }
        return of({
          isSuccess: false,
          error: {
            code: error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao carregar propostas'
          }
        } as Result<PropertyProposal[]>);
      })
    );
  }

  approve(proposalId: string): Observable<Result<boolean>> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${proposalId}/approve`, {}).pipe(
      map(() => {
        return { isSuccess: true, value: true } as Result<boolean>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error approving proposal:', error);
        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao aprovar proposta'
          }
        } as Result<boolean>);
      })
    );
  }

  reject(proposalId: string, reason: string): Observable<Result<boolean>> {
    // Backend expects RejectProposalCommand with ProposalId and RejectionReason
    const command = {
      proposalId: proposalId,
      rejectionReason: reason
    };
    
    return this.http.put<{ message: string }>(`${this.apiUrl}/${proposalId}/reject`, command).pipe(
      map(() => {
        return { isSuccess: true, value: true } as Result<boolean>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error rejecting proposal:', error);
        let errorDescription = 'Erro ao rejeitar proposta';
        
        // Handle validation errors from FluentValidation
        if (error.error?.errors) {
          const validationErrors = error.error.errors;
          const errorMessages: string[] = [];
          Object.keys(validationErrors).forEach(key => {
            const errors = validationErrors[key] as string[];
            if (errors && errors.length > 0) {
              errorMessages.push(errors.join(', '));
            }
          });
          errorDescription = errorMessages.length > 0 ? errorMessages.join(' | ') : errorDescription;
        } else if (error.error?.description) {
          errorDescription = error.error.description;
        } else if (error.error?.message) {
          errorDescription = error.error.message;
        }
        
        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: errorDescription
          }
        } as Result<boolean>);
      })
    );
  }

  addNegotiation(proposalId: string, senderId: string, message: string, counterOffer?: number): Observable<Result<string>> {
    const body: any = {
      proposalId,
      senderId,
      message
    };
    
    // Only include counterOffer if it's provided and greater than 0
    if (counterOffer && counterOffer > 0) {
      body.counterOffer = counterOffer;
    }
    
    return this.http.post<GuidResponse>(`${this.apiUrl}/${proposalId}/negotiate`, body).pipe(
      map((response: GuidResponse) => {
        // Backend returns Guid directly (string or object), wrap it in Result format
        const id = typeof response === 'string' ? response : (response.id || String(response));
        return { isSuccess: true, value: id } as Result<string>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error adding negotiation:', error);
        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao adicionar negociação'
          }
        } as Result<string>);
      })
    );
  }

  updateNegotiationStatus(negotiationId: string, status: string): Observable<Result<boolean>> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/negotiations/${negotiationId}/status`, { status }).pipe(
      map(() => {
        return { isSuccess: true, value: true } as Result<boolean>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error updating negotiation status:', error);
        let errorDescription = 'Erro ao atualizar estado da negociação';

        if (error.error?.description) {
          errorDescription = error.error.description;
        } else if (error.error?.message) {
          errorDescription = error.error.message;
        }

        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: errorDescription
          }
        } as Result<boolean>);
      })
    );
  }

  cancel(proposalId: string): Observable<Result<boolean>> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${proposalId}/cancel`, {}).pipe(
      map(() => {
        return { isSuccess: true, value: true } as Result<boolean>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error cancelling proposal:', error);
        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao cancelar proposta'
          }
        } as Result<boolean>);
      })
    );
  }

  startAnalysis(proposalId: string): Observable<Result<boolean>> {
    return this.http.put<{ message: string }>(`${this.apiUrl}/${proposalId}/start-analysis`, {}).pipe(
      map(() => {
        return { isSuccess: true, value: true } as Result<boolean>;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error('Error starting analysis:', error);
        return of({
          isSuccess: false,
          error: {
            code: error.status === 404 ? 'NOT_FOUND' : error.status === 401 ? 'UNAUTHORIZED' : 'ERROR',
            description: error.error?.description || error.message || 'Erro ao iniciar análise'
          }
        } as Result<boolean>);
      })
    );
  }
}

