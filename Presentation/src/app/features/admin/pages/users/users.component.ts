import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { ToastService } from '../../../../core/services/toast.service';
import { User, UserRole } from '../../../../core/models/user.model';
import { environment } from '../../../../../environments/environment';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { UserDetailModalComponent } from '../../components/user-detail-modal/user-detail-modal.component';

@Component({
  selector: 'app-admin-users',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AdminSidebarComponent, UserDetailModalComponent],
  templateUrl: './users.component.html',
  styleUrl: './users.component.scss'
})
export class AdminUsersComponent implements OnInit {
  users: User[] = [];
  filteredUsers: User[] = [];
  loading: boolean = true;
  
  // Filters
  roleFilter: string = 'all';
  statusFilter: string = 'all';
  searchTerm: string = '';

  // Modal state
  showUserModal: boolean = false;
  selectedUser: User | null = null;

  constructor(
    private http: HttpClient,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.loading = true;
    this.http.get<any>(`${environment.apiUrl}/users`).subscribe({
      next: (result) => {
        if (result.isSuccess && result.value) {
          this.users = result.value;
          this.applyFilters();
        }
        this.loading = false;
      },
      error: () => {
        // Mock data for development
        this.users = [
          {
            id: '1',
            firstName: 'Admin',
            lastName: 'Sistema',
            email: 'admin@dreamluso.pt',
            role: UserRole.Admin,
            isActive: true,
            profileImageUrl: ''
          }
        ];
        this.applyFilters();
        this.loading = false;
      }
    });
  }

  applyFilters(): void {
    this.filteredUsers = this.users.filter(u => {
      const matchesRole = this.roleFilter === 'all' || u.role === this.roleFilter;
      const matchesStatus = this.statusFilter === 'all' || 
        (this.statusFilter === 'active' && u.isActive) ||
        (this.statusFilter === 'inactive' && !u.isActive);
      const matchesSearch = !this.searchTerm || 
        u.firstName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        u.lastName?.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        u.email?.toLowerCase().includes(this.searchTerm.toLowerCase());
      
      return matchesRole && matchesStatus && matchesSearch;
    });
  }

  onFilterChange(): void {
    this.applyFilters();
  }

  toggleUserStatus(userId: string, currentStatus: boolean): void {
    const action = currentStatus ? 'desativar' : 'ativar';
    if (!confirm(`Tem certeza que deseja ${action} este usuário?`)) {
      return;
    }

    this.http.put(`${environment.apiUrl}/users/${userId}/toggle-status`, {}).subscribe({
      next: () => {
        this.toastService.success(`Usuário ${action}do com sucesso!`);
        this.loadUsers();
      },
      error: () => {
        this.toastService.error('Erro ao alterar estado do usuário');
      }
    });
  }

  deleteUser(userId: string, userName: string): void {
    if (!confirm(`Tem certeza que deseja ELIMINAR "${userName}"? Esta ação é irreversível!`)) {
      return;
    }

    this.http.delete(`${environment.apiUrl}/users/${userId}`).subscribe({
      next: () => {
        this.toastService.success('Usuário eliminado com sucesso!');
        this.loadUsers();
      },
      error: () => {
        this.toastService.error('Erro ao eliminar usuário');
      }
    });
  }

  getRoleBadgeClass(role: string): string {
    const baseClass = 'px-3 py-1 rounded-full text-xs font-semibold';
    switch (role) {
      case 'Admin':
        return `${baseClass} bg-purple-100 text-purple-800`;
      case 'RealEstateAgent':
        return `${baseClass} bg-blue-100 text-blue-800`;
      case 'Client':
        return `${baseClass} bg-green-100 text-green-800`;
      default:
        return `${baseClass} bg-gray-100 text-gray-800`;
    }
  }

  getRoleLabel(role: string): string {
    switch (role) {
      case 'Admin': return 'Administrador';
      case 'RealEstateAgent': return 'Agente Imobiliário';
      case 'Client': return 'Cliente';
      default: return role;
    }
  }

  getStatusBadgeClass(isActive: boolean): string {
    return isActive 
      ? 'px-3 py-1 rounded-full text-xs font-semibold bg-green-100 text-green-800'
      : 'px-3 py-1 rounded-full text-xs font-semibold bg-red-100 text-red-800';
  }

  getActiveUsersCount(): number {
    return this.filteredUsers.filter(u => u.isActive).length;
  }

  getInactiveUsersCount(): number {
    return this.filteredUsers.filter(u => !u.isActive).length;
  }

  viewUserDetails(user: User): void {
    this.selectedUser = user;
    this.showUserModal = true;
  }

  closeUserModal(): void {
    this.showUserModal = false;
    this.selectedUser = null;
  }

  toggleUserStatusFromModal(user: User): void {
    this.toggleUserStatus(user.id, user.isActive);
  }

  deleteUserFromModal(user: User): void {
    this.deleteUser(user.id, `${user.firstName} ${user.lastName}`);
  }
}

