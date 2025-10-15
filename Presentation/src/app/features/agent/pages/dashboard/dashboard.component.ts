import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { PropertyService } from '../../../../core/services/property.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { User } from '../../../../core/models/user.model';
import { Property } from '../../../../core/models/property.model';
import { PropertyProposal } from '../../../../core/models/proposal.model';

@Component({
  selector: 'app-agent-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class AgentDashboardComponent implements OnInit {
  currentUser: User | null = null;
  myProperties: Property[] = [];
  receivedProposals: PropertyProposal[] = [];
  loading: boolean = true;

  stats = {
    totalProperties: 0,
    activeProperties: 0,
    totalProposals: 0,
    pendingProposals: 0,
    scheduledVisits: 0
  };

  constructor(
    private authService: AuthService,
    private propertyService: PropertyService,
    private proposalService: ProposalService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadDashboardData();
    }
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load agent's properties
    this.propertyService.getByAgent(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.myProperties = result.value.slice(0, 5); // Latest 5
        this.stats.totalProperties = result.value.length;
        this.stats.activeProperties = result.value.filter(p => p.status === 'Available').length;
      }
      this.loading = false;
    });

    // Load received proposals
    this.proposalService.getByAgent(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.receivedProposals = result.value.slice(0, 5); // Latest 5
        this.stats.totalProposals = result.value.length;
        this.stats.pendingProposals = result.value.filter(
          p => p.status === 'Pending' || p.status === 'UnderAnalysis'
        ).length;
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Available':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Reserved':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Sold':
      case 'Rented':
        return `${baseClass} bg-red-100 text-red-800`;
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }
}

