import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { VisitService } from '../../../../core/services/visit.service';
import { ClientService } from '../../../../core/services/client.service';
import { AuthService } from '../../../../core/services/auth.service';
import { PropertyVisit, VisitStatus } from '../../../../core/models/visit.model';
import { User } from '../../../../core/models/user.model';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';

@Component({
  selector: 'app-client-visits',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ClientSidebarComponent],
  templateUrl: './visits.component.html',
  styleUrl: './visits.component.scss'
})
export class ClientVisitsComponent implements OnInit {
  currentUser: User | null = null;
  clientId: string | null = null;
  visits: PropertyVisit[] = [];
  filteredVisits: PropertyVisit[] = [];
  loading: boolean = true;

  statusFilter: string = 'all';
  searchTerm: string = '';

  constructor(
    private visitService: VisitService,
    private authService: AuthService,
    private clientService: ClientService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.clientService.getByUserId(this.currentUser.id).subscribe({
        next: (client: any) => {
          this.clientId = client?.id || null;
          this.loadVisits();
        },
        error: () => {
          this.loading = false;
        }
      });
    }
  }

  loadVisits(): void {
    if (!this.clientId) return;
    this.loading = true;
    this.visitService.getVisitsByClient(this.clientId).subscribe({
      next: (visits) => {
        this.visits = Array.isArray(visits) ? visits : [];
        this.applyFilters();
        this.loading = false;
      },
      error: () => {
        this.visits = [];
        this.filteredVisits = [];
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredVisits = this.visits.filter(v => {
      const matchesStatus = this.statusFilter === 'all' || v.status === this.statusFilter as VisitStatus;
      const matchesSearch = !this.searchTerm ||
        v.propertyTitle?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        v.agentName?.toLowerCase().includes(this.searchTerm.toLowerCase());
      return matchesStatus && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  getStatusBadgeClass(status: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (status) {
      case 'Pending':
        return `${baseClass} bg-yellow-100 text-yellow-800`;
      case 'Confirmed':
        return `${baseClass} bg-blue-100 text-blue-800`;
      case 'Completed':
        return `${baseClass} bg-green-100 text-green-800`;
      case 'Cancelled':
        return `${baseClass} bg-red-100 text-red-800`;
      case 'NoShow':
        return `${baseClass} bg-gray-100 text-gray-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }
}


