import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProposalService } from '../../../../core/services/proposal.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';

@Component({
  selector: 'app-client-proposal-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ClientSidebarComponent],
  templateUrl: './proposal-detail.component.html',
  styleUrl: './proposal-detail.component.scss'
})
export class ClientProposalDetailComponent implements OnInit {
  proposal?: PropertyProposal;
  loading = true;
  proposalId: string = '';
  negotiationMessage: string = '';
  counterOffer?: number;
  showCancelModal: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.proposalId = this.route.snapshot.paramMap.get('id') || '';
    if (this.proposalId) {
      this.loadProposal();
    }
  }

  loadProposal(): void {
    this.loading = true;
    this.proposalService.getById(this.proposalId).subscribe({
      next: (result: any) => {
        if (result.isSuccess && result.value) {
          this.proposal = result.value;
        } else {
          console.error('Proposta não encontrada ou erro ao carregar:', result);
        }
        this.loading = false;
      },
      error: (error: any) => {
        console.error('Erro ao carregar proposta:', error);
        this.loading = false;
      }
    });
  }

  sendNegotiation(): void {
    if (!this.proposal) return;
    const userId = this.authService.getCurrentUser()?.id || '';
    if (!userId || !this.negotiationMessage.trim()) {
      this.toastService.error('Por favor, preencha a mensagem');
      return;
    }
    
    // Validar mensagem (mínimo 5 caracteres conforme backend)
    const trimmedMessage = this.negotiationMessage.trim();
    if (trimmedMessage.length < 5) {
      this.toastService.error('A mensagem deve ter pelo menos 5 caracteres');
      return;
    }
    
    // Validar contraproposta se fornecida
    if (this.counterOffer !== undefined && this.counterOffer !== null && this.counterOffer <= 0) {
      this.toastService.error('A contraproposta deve ser maior que zero');
      return;
    }
    
    this.proposalService
      .addNegotiation(this.proposal.id, userId, trimmedMessage, this.counterOffer)
      .subscribe({
        next: (result: any) => {
          if (result.isSuccess) {
            this.toastService.success('Negociação enviada com sucesso!');
            this.negotiationMessage = '';
            this.counterOffer = undefined;
            this.loadProposal();
          } else {
            const errorMsg = result.error?.description || 'Erro ao adicionar negociação';
            this.toastService.error(errorMsg);
          }
        },
        error: (error: any) => {
          console.error('Erro ao adicionar negociação:', error);
          const errorMsg = error.error?.description || error.error?.message || 'Erro ao adicionar negociação';
          this.toastService.error(errorMsg);
        }
      });
  }

  getNegotiationStatusBadgeClass(status: string): string {
    const baseClass = 'px-2 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Sent':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-300`;
      case 'Viewed':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-300`;
      case 'Accepted':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-300`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-300`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-900/30 dark:text-gray-300`;
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending':
        return 'Pendente';
      case 'UnderAnalysis':
        return 'Em Análise';
      case 'InNegotiation':
        return 'Em Negociação';
      case 'Approved':
        return 'Aprovada';
      case 'Rejected':
        return 'Rejeitada';
      case 'Cancelled':
        return 'Cancelada';
      case 'Completed':
        return 'Concluída';
      default:
        return status;
    }
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800 dark:bg-yellow-900/30 dark:text-yellow-400`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800 dark:bg-blue-900/30 dark:text-blue-400`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800 dark:bg-purple-900/30 dark:text-purple-400`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800 dark:bg-green-900/30 dark:text-green-400`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800 dark:bg-red-900/30 dark:text-red-400`;
      case 'Cancelled':
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
      case 'Completed':
        return `${baseClass} bg-indigo-100 text-indigo-800 dark:bg-indigo-900/30 dark:text-indigo-400`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800 dark:bg-gray-700 dark:text-gray-300`;
    }
  }

  getNegotiationStatusLabel(status: string): string {
    switch (status) {
      case 'Sent':
        return 'Enviado';
      case 'Viewed':
        return 'Visualizado';
      case 'Accepted':
        return 'Aceito';
      case 'Rejected':
        return 'Rejeitado';
      default:
        return status;
    }
  }

  openCancelModal(): void {
    this.showCancelModal = true;
  }

  closeCancelModal(): void {
    this.showCancelModal = false;
  }

  confirmCancel(): void {
    if (!this.proposal) return;

    this.proposalService.cancel(this.proposal.id).subscribe({
      next: (result: any) => {
        if (result.isSuccess) {
          this.toastService.success('Proposta cancelada com sucesso!');
          this.showCancelModal = false;
          this.loadProposal();
        } else {
          const errorMsg = result.error?.description || 'Erro ao cancelar proposta';
          this.toastService.error(errorMsg);
        }
      },
      error: (error: any) => {
        console.error('Erro ao cancelar proposta:', error);
        const errorMsg = error.error?.description || error.error?.message || 'Erro ao cancelar proposta';
        this.toastService.error(errorMsg);
      }
    });
  }
}


