import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';

@Component({
  selector: 'app-client-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './client-sidebar.component.html',
  styleUrl: './client-sidebar.component.scss'
})
export class ClientSidebarComponent {
  private router = inject(Router);
  private authService = inject(AuthService);

  isSidebarCollapsed = false;
  currentUser = this.authService.getCurrentUser();

  menuItems = [
    { icon: 'dashboard', label: 'Dashboard', route: '/client/dashboard', description: 'Visão geral' },
    { icon: 'proposals', label: 'Propostas', route: '/client/proposals', description: 'Minhas propostas' },
    { icon: 'visits', label: 'Visitas', route: '/client/visits', description: 'Visitas agendadas' },
    { icon: 'favorites', label: 'Favoritos', route: '/client/favorites', description: 'Imóveis favoritos' },
    { icon: 'profile', label: 'Perfil', route: '/client/profile', description: 'Meu perfil' }
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


