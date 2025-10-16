import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-detail-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen && user) {
      <!-- Modal Backdrop -->
      <div class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4" (click)="close()">
        
        <!-- Modal Content -->
        <div class="bg-white dark:bg-dark-800 rounded-3xl shadow-2xl max-w-3xl w-full max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
          
          <!-- Header -->
          <div class="sticky top-0 bg-gradient-to-r from-green-600 to-teal-600 dark:from-green-700 dark:to-teal-700 text-white p-6 rounded-t-3xl shadow-lg z-10">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-4">
                <div class="w-20 h-20 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center text-3xl font-bold ring-4 ring-white/30">
                  {{ user.firstName?.charAt(0) || 'U' }}{{ user.lastName?.charAt(0) || '' }}
                </div>
                <div>
                  <h2 class="text-2xl font-bold">{{ user.firstName }} {{ user.lastName }}</h2>
                  <p class="text-white/80 text-sm">Perfil Completo do Usuário</p>
                </div>
              </div>
              <button (click)="close()" class="w-10 h-10 bg-white/20 hover:bg-white/30 rounded-full flex items-center justify-center transition-all hover:rotate-90 duration-300">
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
          </div>

          <!-- Body -->
          <div class="p-6 space-y-6">
            
            <!-- Status & Role -->
            <div class="flex items-center justify-center gap-3 flex-wrap">
              <span [class]="getRoleBadgeClass(user.role)">
                {{ getRoleLabel(user.role) }}
              </span>
              <span [class]="user.isActive 
                ? 'px-4 py-2 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 rounded-full font-bold text-sm'
                : 'px-4 py-2 bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400 rounded-full font-bold text-sm'">
                {{ user.isActive ? '✅ Ativo' : '❌ Inativo' }}
              </span>
            </div>

            <!-- Contact Info -->
            <div class="bg-gradient-to-br from-blue-50 to-teal-50 dark:from-blue-900/20 dark:to-teal-900/20 rounded-2xl p-6 border border-blue-100 dark:border-blue-800/30">
              <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                <svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                </svg>
                Informações de Contato
              </h3>
              <div class="space-y-3">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/40 rounded-xl flex items-center justify-center flex-shrink-0">
                    <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                    </svg>
                  </div>
                  <div class="flex-1 min-w-0">
                    <p class="text-xs text-gray-500 dark:text-gray-400">Email</p>
                    <p class="font-semibold text-gray-900 dark:text-white break-all">{{ user.email }}</p>
                  </div>
                </div>
                @if (user.phone) {
                  <div class="flex items-center gap-3">
                    <div class="w-10 h-10 bg-green-100 dark:bg-green-900/40 rounded-xl flex items-center justify-center flex-shrink-0">
                      <svg class="w-5 h-5 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
                      </svg>
                    </div>
                    <div class="flex-1">
                      <p class="text-xs text-gray-500 dark:text-gray-400">Telefone</p>
                      <p class="font-semibold text-gray-900 dark:text-white">{{ user.phone || 'Não informado' }}</p>
                    </div>
                  </div>
                }
              </div>
            </div>

            <!-- Additional Info Grid -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <!-- User ID -->
              <div class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl p-4 border border-gray-200 dark:border-gray-700">
                <p class="text-xs text-gray-500 dark:text-gray-400 mb-1 flex items-center gap-2">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 20l4-16m2 16l4-16M6 9h14M4 15h14"/>
                  </svg>
                  ID do Usuário
                </p>
                <p class="font-mono text-sm text-gray-900 dark:text-white font-bold">{{ user.id }}</p>
              </div>

              <!-- Created Date -->
              <div class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl p-4 border border-gray-200 dark:border-gray-700">
                <p class="text-xs text-gray-500 dark:text-gray-400 mb-1 flex items-center gap-2">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                  </svg>
                  Membro desde
                </p>
                <p class="font-semibold text-gray-900 dark:text-white text-sm">{{ (user.createdAt | date:'dd/MM/yyyy') || 'N/A' }}</p>
              </div>

              <!-- Last Updated -->
              @if (user.updatedAt) {
                <div class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl p-4 border border-gray-200 dark:border-gray-700">
                  <p class="text-xs text-gray-500 dark:text-gray-400 mb-1 flex items-center gap-2">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                    </svg>
                    Última Atualização
                  </p>
                  <p class="font-semibold text-gray-900 dark:text-white text-sm">{{ user.updatedAt | date:'dd/MM/yyyy HH:mm' }}</p>
                </div>
              }

              <!-- Account Status -->
              <div class="bg-gray-50 dark:bg-gray-800/50 rounded-2xl p-4 border border-gray-200 dark:border-gray-700">
                <p class="text-xs text-gray-500 dark:text-gray-400 mb-1 flex items-center gap-2">
                  <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Status da Conta
                </p>
                <p class="font-semibold text-sm" [class]="user.isActive ? 'text-green-600 dark:text-green-400' : 'text-red-600 dark:text-red-400'">
                  {{ user.isActive ? 'Ativa e Operacional' : 'Desativada' }}
                </p>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex gap-3 pt-4 border-t border-gray-200 dark:border-gray-700">
              @if (user.isActive) {
                <button 
                  (click)="toggleStatus()"
                  class="flex-1 px-6 py-3.5 bg-gradient-to-r from-orange-500 to-yellow-500 hover:from-orange-600 hover:to-yellow-600 text-white font-bold rounded-full transition-all shadow-lg hover:shadow-xl flex items-center justify-center gap-2">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 9v6m4-6v6m7-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Desativar
                </button>
              } @else {
                <button 
                  (click)="toggleStatus()"
                  class="flex-1 px-6 py-3.5 bg-gradient-to-r from-green-500 to-emerald-500 hover:from-green-600 hover:to-emerald-600 text-white font-bold rounded-full transition-all shadow-lg hover:shadow-xl flex items-center justify-center gap-2">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z"/>
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Ativar
                </button>
              }
              <button 
                (click)="deleteUser()"
                class="px-6 py-3.5 bg-gradient-to-r from-red-500 to-rose-500 hover:from-red-600 hover:to-rose-600 text-white font-bold rounded-full transition-all shadow-lg hover:shadow-xl flex items-center justify-center gap-2">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                </svg>
                Eliminar
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class UserDetailModalComponent {
  @Input() isOpen: boolean = false;
  @Input() user: any = null;
  @Output() closeModal = new EventEmitter<void>();
  @Output() toggleUserStatus = new EventEmitter<any>();
  @Output() deleteUserEvent = new EventEmitter<any>();

  close(): void {
    this.closeModal.emit();
  }

  toggleStatus(): void {
    this.toggleUserStatus.emit(this.user);
    this.close();
  }

  deleteUser(): void {
    this.deleteUserEvent.emit(this.user);
    this.close();
  }

  getRoleBadgeClass(role: string): string {
    const baseClass = 'px-4 py-2 rounded-full font-bold text-sm';
    switch (role) {
      case 'Admin':
        return `${baseClass} bg-purple-100 dark:bg-purple-900/30 text-purple-700 dark:text-purple-400`;
      case 'RealEstateAgent':
        return `${baseClass} bg-blue-100 dark:bg-blue-900/30 text-blue-700 dark:text-blue-400`;
      case 'Client':
        return `${baseClass} bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400`;
      default:
        return `${baseClass} bg-gray-100 dark:bg-gray-700 text-gray-700 dark:text-gray-300`;
    }
  }

  getRoleLabel(role: string): string {
    switch (role) {
      case 'Admin': return '👑 Administrador';
      case 'RealEstateAgent': return '🏢 Agente Imobiliário';
      case 'Client': return '👤 Cliente';
      default: return role;
    }
  }
}

