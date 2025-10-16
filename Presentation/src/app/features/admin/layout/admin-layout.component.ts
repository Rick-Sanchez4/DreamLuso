import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AdminSidebarComponent } from '../components/admin-sidebar/admin-sidebar.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent],
  template: `
    <div class="flex min-h-screen bg-gradient-to-br from-gray-50 via-white to-green-50/20 dark:from-dark-950 dark:via-dark-900 dark:to-green-950">
      <app-admin-sidebar></app-admin-sidebar>
      <main class="flex-1 ml-72 transition-all duration-300">
        <router-outlet></router-outlet>
      </main>
    </div>
  `
})
export class AdminLayoutComponent {}

