import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProposalService } from '../../../../core/services/proposal.service';
import { AuthService } from '../../../../core/services/auth.service';
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

  constructor(
    private route: ActivatedRoute,
    private proposalService: ProposalService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.proposalId = this.route.snapshot.paramMap.get('id') || '';
    if (this.proposalId) {
      this.loadProposal();
    }
  }

  loadProposal(): void {
    this.loading = true;
    this.proposalService.getById(this.proposalId).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.proposal = result.value;
      }
      this.loading = false;
    });
  }

  sendNegotiation(): void {
    if (!this.proposal) return;
    const userId = this.authService.getCurrentUser()?.id || '';
    if (!userId || !this.negotiationMessage.trim()) return;
    this.proposalService
      .addNegotiation(this.proposal.id, userId, this.negotiationMessage.trim(), this.counterOffer)
      .subscribe(() => {
        this.negotiationMessage = '';
        this.counterOffer = undefined;
        this.loadProposal();
      });
  }
}


