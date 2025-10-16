import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { AgentDetailModalComponent } from '../../components/agent-detail-modal/agent-detail-modal.component';
import { environment } from '../../../../../environments/environment';
import { ToastService } from '../../../../core/services/toast.service';

@Component({
  selector: 'app-admin-agents',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent, AgentDetailModalComponent],
  templateUrl: './agents.component.html',
  styleUrl: './agents.component.scss'
})
export class AdminAgentsComponent implements OnInit {
  agents: any[] = [];
  filteredAgents: any[] = [];
  pendingAgents: any[] = [];
  loading: boolean = true;
  searchTerm: string = '';
  showPending: boolean = false; // Show approved agents by default

  // Modal state
  showAgentModal: boolean = false;
  selectedAgent: any = null;

  // Computed stats
  get totalProperties(): number {
    return this.agents.reduce((sum, a) => sum + (a.properties || a.totalListings || 0), 0);
  }

  get totalSales(): number {
    return this.agents.reduce((sum, a) => sum + (a.sales || a.totalSales || 0), 0);
  }

  get averageRating(): number {
    const agentsWithRating = this.agents.filter(a => a.rating > 0);
    if (agentsWithRating.length === 0) return 0;
    const sum = agentsWithRating.reduce((total, a) => total + a.rating, 0);
    return Math.round((sum / agentsWithRating.length) * 100) / 100;
  }

  constructor(
    private http: HttpClient,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadAgents();
  }

  loadAgents(): void {
    this.loading = true;
    
    // Try to load from API
    this.http.get<any>(`${environment.apiUrl}/agents`).subscribe({
      next: (result) => {
        if (result && result.agents) {
          this.agents = result.agents.map((a: any) => ({
            id: a.id,
            name: a.fullName || 'Agente',
            email: a.email || '',
            phone: a.phone || 'Não informado',
            licenseNumber: a.licenseNumber || '',
            specialization: a.specialization || '',
            properties: a.totalListings || 0,
            sales: a.totalSales || 0,
            rating: a.rating || 0,
            isActive: a.isActive || false
          }));
        } else {
          this.useMockData();
        }
        this.updateLists();
        this.loading = false;
      },
      error: () => {
        this.useMockData();
        this.updateLists();
        this.loading = false;
      }
    });
  }

  useMockData(): void {
    this.agents = [
      { id: 1, name: 'Carlos Mendes', email: 'carlos@dreamluso.pt', phone: '+351 93 456 7890', properties: 25, sales: 18, rating: 4.8, isActive: true },
      { id: 2, name: 'Ana Pereira', email: 'ana@dreamluso.pt', phone: '+351 94 567 8901', properties: 32, sales: 24, rating: 4.9, isActive: true },
      { id: 3, name: 'Pedro Santos', email: 'pedro@dreamluso.pt', phone: '+351 95 678 9012', properties: 0, sales: 0, rating: 0, isActive: false },
      { id: 4, name: 'Rita Silva', email: 'rita@dreamluso.pt', phone: '+351 96 789 0123', properties: 0, sales: 0, rating: 0, isActive: false }
    ];
  }

  updateLists(): void {
    // IsActive = false means pending approval, IsActive = true means approved
    this.pendingAgents = this.agents.filter(a => !a.isActive);
    const baseList = this.showPending ? this.pendingAgents : this.agents.filter(a => a.isActive);
    this.filteredAgents = [...baseList];
  }

  onSearch(): void {
    const baseList = this.showPending ? this.pendingAgents : this.agents.filter(a => a.isActive);
    
    if (!this.searchTerm.trim()) {
      this.filteredAgents = [...baseList];
      return;
    }
    
    const term = this.searchTerm.toLowerCase();
    this.filteredAgents = baseList.filter(agent =>
      agent.name.toLowerCase().includes(term) ||
      agent.email.toLowerCase().includes(term)
    );
  }

  toggleView(showPending: boolean): void {
    this.showPending = showPending;
    this.searchTerm = '';
    this.onSearch();
  }

  viewAgentProfile(agent: any): void {
    this.selectedAgent = agent;
    this.showAgentModal = true;
  }

  closeAgentModal(): void {
    this.showAgentModal = false;
    this.selectedAgent = null;
  }

  approveAgentFromModal(agent: any): void {
    this.approveAgent(agent.id);
  }

  rejectAgentFromModal(agent: any): void {
    this.rejectAgent(agent.id);
  }

  approveAgent(agentId: number): void {
    // TODO: Call API to approve agent (set isActive = true)
    const agent = this.agents.find(a => a.id === agentId);
    if (agent) {
      agent.isActive = true;
      this.updateLists();
      this.toastService.success('Agente aprovado com sucesso!');
    }
  }

  rejectAgent(agentId: number): void {
    if (!confirm('Tem certeza que deseja rejeitar este agente?')) {
      return;
    }
    
    // TODO: Call API to reject/delete agent
    this.agents = this.agents.filter(a => a.id !== agentId);
    this.updateLists();
    this.toastService.success('Agente rejeitado!');
  }
}

