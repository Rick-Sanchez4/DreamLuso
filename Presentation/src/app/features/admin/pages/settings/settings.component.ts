import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { ThemeService } from '../../../../core/services/theme.service';

type SettingsCategory = 'general' | 'security' | 'notifications' | 'payments' | 'personalization';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent, FormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class AdminSettingsComponent {
  activeCategory: SettingsCategory = 'general';

  constructor(public themeService: ThemeService) {}

  // General Settings
  siteName: string = 'DreamLuso';
  siteDescription: string = 'Plataforma de imóveis em Portugal';
  maintenanceMode: boolean = false;

  // Security Settings
  twoFactorAuth: boolean = false;
  sessionTimeout: number = 30;
  passwordExpiry: number = 90;

  // Notification Settings
  emailNotifications: boolean = true;
  smsNotifications: boolean = false;
  pushNotifications: boolean = true;

  // Payment Settings
  currency: string = 'EUR';
  taxRate: number = 23;
  paymentMethods: string[] = ['credit_card', 'mbway', 'bank_transfer'];

  // Personalization Settings
  theme: string = 'light';
  language: string = 'pt';
  itemsPerPage: number = 20;

  setActiveCategory(category: SettingsCategory): void {
    this.activeCategory = category;
  }

  isActiveCategory(category: SettingsCategory): boolean {
    return this.activeCategory === category;
  }

  saveSettings(): void {
    // TODO: Implement save logic
    console.log('Settings saved:', {
      general: {
        siteName: this.siteName,
        siteDescription: this.siteDescription,
        maintenanceMode: this.maintenanceMode
      },
      security: {
        twoFactorAuth: this.twoFactorAuth,
        sessionTimeout: this.sessionTimeout,
        passwordExpiry: this.passwordExpiry
      },
      notifications: {
        emailNotifications: this.emailNotifications,
        smsNotifications: this.smsNotifications,
        pushNotifications: this.pushNotifications
      },
      payments: {
        currency: this.currency,
        taxRate: this.taxRate,
        paymentMethods: this.paymentMethods
      },
      personalization: {
        theme: this.theme,
        language: this.language,
        itemsPerPage: this.itemsPerPage
      }
    });
    
    alert('Configurações salvas com sucesso!');
  }

  cancelChanges(): void {
    // TODO: Implement cancel logic (reset to saved values)
    console.log('Changes cancelled');
  }
}

