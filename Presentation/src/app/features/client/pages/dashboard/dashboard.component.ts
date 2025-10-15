import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ProposalService } from '../../../../core/services/proposal.service';
import { NotificationService } from '../../../../core/services/notification.service';
import { PropertyProposal } from '../../../../core/models/proposal.model';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-client-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class ClientDashboardComponent implements OnInit {
  currentUser: User | null = null;
  myProposals: PropertyProposal[] = [];
  unreadNotifications: number = 0;
  loading: boolean = true;

  stats = {
    totalProposals: 0,
    pendingProposals: 0,
    approvedProposals: 0,
    scheduledVisits: 0
  };

  constructor(
    private authService: AuthService,
    private proposalService: ProposalService,
    private notificationService: NotificationService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadDashboardData();
    }
  }

  loadDashboardData(): void {
    this.loading = true;

    // Load proposals
    this.proposalService.getByClient(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.myProposals = result.value.slice(0, 5); // Latest 5
        this.stats.totalProposals = result.value.length;
        this.stats.pendingProposals = result.value.filter(p => p.status === 'Pending' || p.status === 'UnderAnalysis').length;
        this.stats.approvedProposals = result.value.filter(p => p.status === 'Approved').length;
      }
      this.loading = false;
    });

    // Load unread notifications
    this.notificationService.getUnreadCount(this.currentUser!.id).subscribe(result => {
      if (result.isSuccess && result.value) {
        this.unreadNotifications = result.value.unreadCount;
      }
    });
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'UnderAnalysis':
        return `${baseClass} bg-blue-100 text-blue-800`;
      case 'InNegotiation':
        return `${baseClass} bg-purple-100 text-purple-800`;
      case 'Approved':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Rejected':
        return `${baseClass} bg-red-100 text-red-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }
}

