import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-agent-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './agent-sidebar.component.html',
  styleUrl: './agent-sidebar.component.scss'
})
export class AgentSidebarComponent {
  private router = inject(Router);
  private authService = inject(AuthService);
  
  isSidebarCollapsed = false;
  currentUser = this.authService.getCurrentUser();

  menuItems = [
    {
      icon: 'dashboard',
      label: 'Dashboard',
      route: '/agent/dashboard',
      description: 'Visão geral'
    },
    {
      icon: 'properties',
      label: 'Meus Imóveis',
      route: '/agent/properties',
      description: 'Gerir imóveis'
    },
    {
      icon: 'proposals',
      label: 'Propostas',
      route: '/agent/proposals',
      description: 'Propostas recebidas'
    },
    {
      icon: 'visits',
      label: 'Visitas',
      route: '/agent/visits',
      description: 'Visitas agendadas'
    },
    {
      icon: 'contracts',
      label: 'Contratos',
      route: '/agent/contracts',
      description: 'Contratos ativos'
    },
    {
      icon: 'profile',
      label: 'Perfil',
      route: '/agent/profile',
      description: 'Meu perfil'
    }
  ];

  toggleSidebar(): void {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  isActiveRoute(route: string): boolean {
    return this.router.url.includes(route);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/']);
  }
}

