import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-sidebar.component.html',
  styleUrl: './admin-sidebar.component.scss'
})
export class AdminSidebarComponent {
  private router = inject(Router);
  private authService = inject(AuthService);
  
  isSidebarCollapsed = false;
  currentUser = this.authService.getCurrentUser();

  menuItems = [
    {
      icon: 'dashboard',
      label: 'Dashboard',
      route: '/admin/dashboard',
      description: 'Visão geral'
    },
    {
      icon: 'users',
      label: 'Usuários',
      route: '/admin/users',
      description: 'Gerir usuários'
    },
    {
      icon: 'clients',
      label: 'Clientes',
      route: '/admin/clients',
      description: 'Gestão de clientes'
    },
    {
      icon: 'agents',
      label: 'Agentes',
      route: '/admin/agents',
      description: 'Agentes imobiliários'
    },
    {
      icon: 'properties',
      label: 'Imóveis',
      route: '/admin/properties',
      description: 'Gerir imóveis'
    },
    {
      icon: 'proposals',
      label: 'Propostas',
      route: '/admin/proposals',
      description: 'Monitorizar propostas'
    },
    {
      icon: 'comments',
      label: 'Comentários',
      route: '/admin/comments',
      description: 'Moderar comentários'
    },
    {
      icon: 'analytics',
      label: 'Analytics',
      route: '/admin/analytics',
      description: 'Relatórios e gráficos'
    },
    {
      icon: 'settings',
      label: 'Configurações',
      route: '/admin/settings',
      description: 'Definições do sistema'
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

