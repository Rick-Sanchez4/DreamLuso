import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-agent-detail-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen && agent) {
      <!-- Modal Backdrop -->
      <div class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4" (click)="close()">
        
        <!-- Modal Content -->
        <div class="bg-white dark:bg-dark-800 rounded-3xl shadow-2xl max-w-3xl w-full max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
          
          <!-- Header -->
          <div class="sticky top-0 bg-gradient-to-r from-green-600 to-teal-600 text-white p-6 rounded-t-3xl">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-4">
                <div class="w-20 h-20 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center text-3xl font-bold">
                  {{ agent.name.charAt(0) }}
                </div>
                <div>
                  <h2 class="text-2xl font-bold">{{ agent.name }}</h2>
                  <div class="flex items-center gap-2 mt-1">
                    @if (agent.isActive) {
                      <span class="px-3 py-1 bg-emerald-500/30 backdrop-blur-sm rounded-full text-xs font-bold">✅ Aprovado</span>
                    } @else {
                      <span class="px-3 py-1 bg-orange-500/30 backdrop-blur-sm rounded-full text-xs font-bold">⏳ Pendente</span>
                    }
                    @if (agent.isActive && agent.rating > 0) {
                      <div class="flex items-center gap-1 px-3 py-1 bg-white/20 backdrop-blur-sm rounded-full">
                        <span class="text-yellow-300">⭐</span>
                        <span class="text-sm font-bold">{{ agent.rating }}</span>
                      </div>
                    }
                  </div>
                </div>
              </div>
              <button (click)="close()" class="w-10 h-10 bg-white/20 hover:bg-white/30 rounded-full flex items-center justify-center transition-all">
                <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
          </div>

          <!-- Body -->
          <div class="p-6 space-y-6">
            
            <!-- Stats (only for active agents) -->
            @if (agent.isActive) {
              <div class="grid grid-cols-3 gap-4">
                <div class="bg-gradient-to-br from-green-50 to-emerald-50 dark:from-green-900/20 dark:to-emerald-900/20 rounded-2xl p-5 text-center border border-green-100 dark:border-green-800/30">
                  <div class="w-12 h-12 bg-green-500 rounded-xl flex items-center justify-center mx-auto mb-3">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                    </svg>
                  </div>
                  <p class="text-3xl font-bold text-green-600 dark:text-green-400">{{ agent.properties }}</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Imóveis Listados</p>
                </div>
                <div class="bg-gradient-to-br from-blue-50 to-indigo-50 dark:from-blue-900/20 dark:to-indigo-900/20 rounded-2xl p-5 text-center border border-blue-100 dark:border-blue-800/30">
                  <div class="w-12 h-12 bg-blue-500 rounded-xl flex items-center justify-center mx-auto mb-3">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                    </svg>
                  </div>
                  <p class="text-3xl font-bold text-blue-600 dark:text-blue-400">{{ agent.sales }}</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Vendas Realizadas</p>
                </div>
                <div class="bg-gradient-to-br from-yellow-50 to-orange-50 dark:from-yellow-900/20 dark:to-orange-900/20 rounded-2xl p-5 text-center border border-yellow-100 dark:border-yellow-800/30">
                  <div class="w-12 h-12 bg-yellow-500 rounded-xl flex items-center justify-center mx-auto mb-3">
                    <svg class="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.049 2.927c.3-.921 1.603-.921 1.902 0l1.519 4.674a1 1 0 00.95.69h4.915c.969 0 1.371 1.24.588 1.81l-3.976 2.888a1 1 0 00-.363 1.118l1.518 4.674c.3.922-.755 1.688-1.538 1.118l-3.976-2.888a1 1 0 00-1.176 0l-3.976 2.888c-.783.57-1.838-.197-1.538-1.118l1.518-4.674a1 1 0 00-.363-1.118l-3.976-2.888c-.784-.57-.38-1.81.588-1.81h4.914a1 1 0 00.951-.69l1.519-4.674z"/>
                    </svg>
                  </div>
                  <p class="text-3xl font-bold text-yellow-600 dark:text-yellow-400">{{ agent.rating }}</p>
                  <p class="text-xs text-gray-600 dark:text-gray-400 mt-1">Rating Médio</p>
                </div>
              </div>
            }

            <!-- License & Professional Info -->
            @if (agent.licenseNumber || agent.specialization) {
              <div class="bg-gradient-to-br from-teal-50 to-emerald-50 dark:from-teal-900/20 dark:to-emerald-900/20 rounded-2xl p-6 border border-teal-100 dark:border-teal-800/30">
                <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                  <svg class="w-5 h-5 text-teal-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                  </svg>
                  Informações Profissionais
                </h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  @if (agent.licenseNumber) {
                    <div class="flex items-center gap-3 p-3 bg-white/50 dark:bg-white/5 rounded-xl">
                      <div class="w-10 h-10 bg-teal-100 dark:bg-teal-900/40 rounded-lg flex items-center justify-center">
                        <svg class="w-5 h-5 text-teal-600 dark:text-teal-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                        </svg>
                      </div>
                      <div>
                        <p class="text-xs text-gray-500 dark:text-gray-400">Licença AMI</p>
                        <p class="font-semibold text-gray-900 dark:text-white text-sm">{{ agent.licenseNumber }}</p>
                      </div>
                    </div>
                  }
                  @if (agent.specialization) {
                    <div class="flex items-center gap-3 p-3 bg-white/50 dark:bg-white/5 rounded-xl">
                      <div class="w-10 h-10 bg-emerald-100 dark:bg-emerald-900/40 rounded-lg flex items-center justify-center">
                        <svg class="w-5 h-5 text-emerald-600 dark:text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z"/>
                        </svg>
                      </div>
                      <div>
                        <p class="text-xs text-gray-500 dark:text-gray-400">Especialização</p>
                        <p class="font-semibold text-gray-900 dark:text-white text-sm">{{ agent.specialization }}</p>
                      </div>
                    </div>
                  }
                </div>
              </div>
            }

            <!-- Contact Info -->
            <div class="bg-gradient-to-br from-blue-50 to-cyan-50 dark:from-blue-900/20 dark:to-cyan-900/20 rounded-2xl p-6 border border-blue-100 dark:border-blue-800/30">
              <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                <svg class="w-5 h-5 text-blue-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                </svg>
                Informações de Contato
              </h3>
              <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div class="flex items-center gap-3 p-3 bg-white/50 dark:bg-white/5 rounded-xl">
                  <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/40 rounded-lg flex items-center justify-center">
                    <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-gray-500 dark:text-gray-400">Email</p>
                    <p class="font-semibold text-gray-900 dark:text-white text-sm truncate">{{ agent.email }}</p>
                  </div>
                </div>
                <div class="flex items-center gap-3 p-3 bg-white/50 dark:bg-white/5 rounded-xl">
                  <div class="w-10 h-10 bg-green-100 dark:bg-green-900/40 rounded-lg flex items-center justify-center">
                    <svg class="w-5 h-5 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-gray-500 dark:text-gray-400">Telefone</p>
                    <p class="font-semibold text-gray-900 dark:text-white text-sm">{{ agent.phone }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Actions -->
            @if (!agent.isActive) {
              <div class="bg-orange-50 dark:bg-orange-900/20 rounded-2xl p-6 border border-orange-200 dark:border-orange-800/30">
                <p class="text-sm text-gray-700 dark:text-gray-300 mb-4 text-center">Este agente está aguardando aprovação para acessar a plataforma.</p>
                <div class="flex gap-3">
                  <button 
                    (click)="approve()"
                    class="flex-1 px-6 py-3 bg-green-500 hover:bg-green-600 text-white font-bold rounded-full transition-all shadow-lg">
                    ✅ Aprovar Agente
                  </button>
                  <button 
                    (click)="reject()"
                    class="flex-1 px-6 py-3 bg-red-500 hover:bg-red-600 text-white font-bold rounded-full transition-all shadow-lg">
                    ❌ Rejeitar
                  </button>
                </div>
              </div>
            } @else {
              <div class="flex gap-3">
                <button class="flex-1 px-6 py-3 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-full transition-all shadow-lg">
                  📧 Enviar Email
                </button>
                <button class="flex-1 px-6 py-3 bg-teal-500 hover:bg-teal-600 text-white font-bold rounded-full transition-all shadow-lg">
                  🏠 Ver Imóveis ({{ agent.properties }})
                </button>
              </div>
            }
          </div>
        </div>
      </div>
    }
  `
})
export class AgentDetailModalComponent {
  @Input() isOpen: boolean = false;
  @Input() agent: any = null;
  @Output() closeModal = new EventEmitter<void>();
  @Output() approveAgent = new EventEmitter<any>();
  @Output() rejectAgent = new EventEmitter<any>();

  close(): void {
    this.closeModal.emit();
  }

  approve(): void {
    this.approveAgent.emit(this.agent);
    this.close();
  }

  reject(): void {
    this.rejectAgent.emit(this.agent);
    this.close();
  }
}

