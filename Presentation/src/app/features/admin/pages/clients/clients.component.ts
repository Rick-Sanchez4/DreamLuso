import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { ClientDetailModalComponent } from '../../components/client-detail-modal/client-detail-modal.component';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-admin-clients',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent, ClientDetailModalComponent],
  templateUrl: './clients.component.html',
  styleUrl: './clients.component.scss'
})
export class AdminClientsComponent implements OnInit {
  clients: any[] = [];
  filteredClients: any[] = [];
  loading: boolean = true;
  searchTerm: string = '';
  
  // Modal state
  showClientModal: boolean = false;
  selectedClient: any = null;

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.loading = true;
    
    // Try to load from API
    this.http.get<any>(`${environment.apiUrl}/clients`).subscribe({
      next: (result) => {
        if (result && result.clients) {
          this.clients = result.clients.map((c: any) => ({
            id: c.id,
            name: c.fullName || 'Cliente',
            email: c.email || '',
            phone: c.phone || 'Não informado',
            nif: c.nif || '',
            properties: 0, // TODO: Get from API when available
            status: c.isActive ? 'active' : 'inactive'
          }));
        } else {
          this.useMockData();
        }
        this.filteredClients = [...this.clients];
        this.loading = false;
      },
      error: () => {
        this.useMockData();
        this.loading = false;
      }
    });
  }

  useMockData(): void {
    this.clients = [
      { id: 1, name: 'Maria Costa', email: 'maria@example.com', phone: '+351 91 234 5678', properties: 3, status: 'active' },
      { id: 2, name: 'João Silva', email: 'joao@example.com', phone: '+351 92 345 6789', properties: 1, status: 'active' },
      { id: 3, name: 'Ana Pereira', email: 'ana@example.com', phone: '+351 93 456 7890', properties: 2, status: 'active' }
    ];
  }

  onSearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredClients = [...this.clients];
      return;
    }
    
    const term = this.searchTerm.toLowerCase();
    this.filteredClients = this.clients.filter(client =>
      client.name.toLowerCase().includes(term) ||
      client.email.toLowerCase().includes(term)
    );
  }

  viewClientDetails(client: any): void {
    this.selectedClient = client;
    this.showClientModal = true;
  }

  closeClientModal(): void {
    this.showClientModal = false;
    this.selectedClient = null;
  }
}

