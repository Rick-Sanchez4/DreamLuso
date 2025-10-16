import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-client-detail-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen && client) {
      <!-- Modal Backdrop -->
      <div class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4" (click)="close()">
        
        <!-- Modal Content -->
        <div class="bg-white dark:bg-dark-800 rounded-3xl shadow-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
          
          <!-- Header -->
          <div class="sticky top-0 bg-gradient-to-r from-green-500 to-teal-500 text-white p-6 rounded-t-3xl">
            <div class="flex items-center justify-between">
              <div class="flex items-center gap-4">
                <div class="w-16 h-16 bg-white/20 backdrop-blur-sm rounded-full flex items-center justify-center text-2xl font-bold">
                  {{ client.name.charAt(0) }}
                </div>
                <div>
                  <h2 class="text-2xl font-bold">{{ client.name }}</h2>
                  <p class="text-white/80 text-sm">Detalhes do Cliente</p>
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
            
            <!-- Status Badge -->
            <div class="flex items-center justify-center">
              <span class="px-6 py-2 bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400 rounded-full font-bold text-sm">
                ✅ Cliente Ativo
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
                  <div class="w-10 h-10 bg-blue-100 dark:bg-blue-900/40 rounded-xl flex items-center justify-center">
                    <svg class="w-5 h-5 text-blue-600 dark:text-blue-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-gray-500 dark:text-gray-400">Email</p>
                    <p class="font-semibold text-gray-900 dark:text-white">{{ client.email }}</p>
                  </div>
                </div>
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 bg-green-100 dark:bg-green-900/40 rounded-xl flex items-center justify-center">
                    <svg class="w-5 h-5 text-green-600 dark:text-green-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z"/>
                    </svg>
                  </div>
                  <div>
                    <p class="text-xs text-gray-500 dark:text-gray-400">Telefone</p>
                    <p class="font-semibold text-gray-900 dark:text-white">{{ client.phone }}</p>
                  </div>
                </div>
              </div>
            </div>

            <!-- Properties Info -->
            <div class="bg-gradient-to-br from-purple-50 to-pink-50 dark:from-purple-900/20 dark:to-pink-900/20 rounded-2xl p-6 border border-purple-100 dark:border-purple-800/30">
              <h3 class="text-lg font-bold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
                <svg class="w-5 h-5 text-purple-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                </svg>
                Imóveis Associados
              </h3>
              <div class="text-center py-4">
                <p class="text-4xl font-bold text-purple-600 dark:text-purple-400">{{ client.properties }}</p>
                <p class="text-sm text-gray-600 dark:text-gray-400 mt-2">Propriedades vinculadas</p>
              </div>
            </div>

            <!-- Actions -->
            <div class="flex gap-3">
              <button class="w-full px-6 py-3 bg-blue-500 hover:bg-blue-600 text-white font-bold rounded-full transition-all">
                📧 Enviar Email
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class ClientDetailModalComponent {
  @Input() isOpen: boolean = false;
  @Input() client: any = null;
  @Output() closeModal = new EventEmitter<void>();

  close(): void {
    this.closeModal.emit();
  }
}

