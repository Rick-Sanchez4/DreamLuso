import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirmation-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen) {
      <!-- Modal Backdrop -->
      <div class="fixed inset-0 bg-black/70 backdrop-blur-sm z-[60] flex items-center justify-center p-4 animate-fade-in">
        
        <!-- Modal Content -->
        <div class="bg-white dark:bg-dark-800 rounded-3xl shadow-2xl max-w-md w-full animate-scale-in" (click)="$event.stopPropagation()">
          
          <!-- Header -->
          <div [class]="getHeaderClass()">
            <div class="flex items-center justify-center mb-3">
              <div [class]="getIconContainerClass()">
                <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24" [innerHTML]="getIconSvg()"></svg>
              </div>
            </div>
            <h3 class="text-xl font-bold text-center">{{ title }}</h3>
          </div>

          <!-- Body -->
          <div class="p-6">
            <p class="text-center text-gray-700 dark:text-gray-300 mb-6">{{ message }}</p>

            <!-- Actions -->
            <div class="flex gap-3">
              <button 
                (click)="onCancel()"
                class="flex-1 px-6 py-3 bg-gray-200 dark:bg-gray-700 hover:bg-gray-300 dark:hover:bg-gray-600 text-gray-900 dark:text-white font-bold rounded-full transition-all">
                {{ cancelText }}
              </button>
              <button 
                (click)="onConfirm()"
                [class]="getConfirmButtonClass()">
                {{ confirmText }}
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    @keyframes fade-in {
      from { opacity: 0; }
      to { opacity: 1; }
    }
    
    @keyframes scale-in {
      from { transform: scale(0.9); opacity: 0; }
      to { transform: scale(1); opacity: 1; }
    }
    
    .animate-fade-in {
      animation: fade-in 0.2s ease-out;
    }
    
    .animate-scale-in {
      animation: scale-in 0.3s ease-out;
    }
  `]
})
export class ConfirmationModalComponent {
  @Input() isOpen: boolean = false;
  @Input() title: string = 'Confirmar Ação';
  @Input() message: string = 'Tem certeza que deseja continuar?';
  @Input() type: 'warning' | 'danger' | 'info' = 'warning';
  @Input() confirmText: string = 'Confirmar';
  @Input() cancelText: string = 'Cancelar';
  
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();

  onConfirm(): void {
    this.confirm.emit();
  }

  onCancel(): void {
    this.cancel.emit();
  }

  getHeaderClass(): string {
    const baseClass = 'p-6 rounded-t-3xl text-white';
    switch (this.type) {
      case 'danger':
        return `${baseClass} bg-gradient-to-r from-red-600 to-rose-600`;
      case 'warning':
        return `${baseClass} bg-gradient-to-r from-orange-500 to-yellow-500`;
      case 'info':
        return `${baseClass} bg-gradient-to-r from-blue-500 to-cyan-500`;
      default:
        return `${baseClass} bg-gradient-to-r from-orange-500 to-yellow-500`;
    }
  }

  getIconContainerClass(): string {
    const baseClass = 'w-16 h-16 rounded-full flex items-center justify-center';
    switch (this.type) {
      case 'danger':
        return `${baseClass} bg-red-100 dark:bg-red-900/30`;
      case 'warning':
        return `${baseClass} bg-orange-100 dark:bg-orange-900/30`;
      case 'info':
        return `${baseClass} bg-blue-100 dark:bg-blue-900/30`;
      default:
        return `${baseClass} bg-orange-100 dark:bg-orange-900/30`;
    }
  }

  getIconSvg(): string {
    switch (this.type) {
      case 'danger':
        return '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>';
      case 'warning':
        return '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>';
      case 'info':
        return '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>';
      default:
        return '<path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>';
    }
  }

  getConfirmButtonClass(): string {
    const baseClass = 'flex-1 px-6 py-3 font-bold rounded-full transition-all';
    switch (this.type) {
      case 'danger':
        return `${baseClass} bg-gradient-to-r from-red-500 to-rose-500 hover:from-red-600 hover:to-rose-600 text-white shadow-lg hover:shadow-xl`;
      case 'warning':
        return `${baseClass} bg-gradient-to-r from-orange-500 to-yellow-500 hover:from-orange-600 hover:to-yellow-600 text-white shadow-lg hover:shadow-xl`;
      case 'info':
        return `${baseClass} bg-gradient-to-r from-blue-500 to-cyan-500 hover:from-blue-600 hover:to-cyan-600 text-white shadow-lg hover:shadow-xl`;
      default:
        return `${baseClass} bg-gradient-to-r from-orange-500 to-yellow-500 hover:from-orange-600 hover:to-yellow-600 text-white shadow-lg hover:shadow-xl`;
    }
  }
}

