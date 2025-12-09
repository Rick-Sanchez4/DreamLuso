import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { AdminSidebarComponent } from '../../components/admin-sidebar/admin-sidebar.component';
import { ThemeService } from '../../../../core/services/theme.service';
import { SettingsService, AppSettings } from '../../../../core/services/settings.service';
import { ToastService } from '../../../../core/services/toast.service';

type SettingsCategory = 'general' | 'security' | 'notifications' | 'payments' | 'personalization';

@Component({
  selector: 'app-admin-settings',
  standalone: true,
  imports: [CommonModule, RouterModule, AdminSidebarComponent, FormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.scss'
})
export class AdminSettingsComponent implements OnInit, OnDestroy {
  activeCategory: SettingsCategory = 'general';
  private settingsSubscription?: Subscription;
  loading: boolean = false;
  saving: boolean = false;

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

  // Store original settings for cancel
  private originalSettings: AppSettings | null = null;

  constructor(
    public themeService: ThemeService,
    private settingsService: SettingsService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    // Load settings from service
    this.settingsSubscription = this.settingsService.settings$.subscribe(settings => {
      this.loadSettingsIntoComponent(settings);
      if (!this.originalSettings) {
        this.originalSettings = { ...settings };
      }
    });
  }

  ngOnDestroy(): void {
    this.settingsSubscription?.unsubscribe();
  }

  private loadSettingsIntoComponent(settings: AppSettings | any): void {
    // General
    this.siteName = settings.general.siteName;
    this.siteDescription = settings.general.siteDescription;
    this.maintenanceMode = settings.general.maintenanceMode;

    // Security
    this.twoFactorAuth = settings.security.twoFactorAuth;
    this.sessionTimeout = settings.security.sessionTimeout;
    this.passwordExpiry = settings.security.passwordExpiry;

    // Notifications
    this.emailNotifications = settings.notifications.emailNotifications;
    this.smsNotifications = settings.notifications.smsNotifications;
    this.pushNotifications = settings.notifications.pushNotifications;

    // Payments
    this.currency = settings.payments.currency;
    this.taxRate = settings.payments.taxRate;
    this.paymentMethods = [...settings.payments.paymentMethods];

    // Personalization
    this.theme = settings.personalization.theme;
    this.language = settings.personalization.language;
    this.itemsPerPage = settings.personalization.itemsPerPage;
  }

  private getCurrentSettings(): AppSettings {
    return {
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
        paymentMethods: [...this.paymentMethods]
      },
      personalization: {
        theme: this.theme,
        language: this.language,
        itemsPerPage: this.itemsPerPage
      }
    };
  }

  setActiveCategory(category: SettingsCategory): void {
    this.activeCategory = category;
  }

  isActiveCategory(category: SettingsCategory): boolean {
    return this.activeCategory === category;
  }

  saveSettings(): void {
    if (this.saving) return;

    const currentSettings = this.getCurrentSettings();
    
    // Validate settings
    if (!this.validateSettings(currentSettings)) {
      return;
    }

    this.saving = true;
    this.settingsService.saveSettings(currentSettings).subscribe({
      next: (success: boolean) => {
        if (success) {
          this.originalSettings = { ...currentSettings };
        }
        this.saving = false;
      },
      error: (error: any) => {
        console.error('Error saving settings:', error);
        this.toastService.error('Erro ao salvar configurações');
        this.saving = false;
      }
    });
  }

  cancelChanges(): void {
    if (this.originalSettings) {
      this.loadSettingsIntoComponent(this.originalSettings);
      this.toastService.info('Alterações canceladas');
    } else {
      // If no original settings, reload from service
      const current = this.settingsService.getSettings();
      this.loadSettingsIntoComponent(current);
      this.toastService.info('Alterações canceladas');
    }
  }

  private validateSettings(settings: AppSettings): boolean {
    // Validate general settings
    if (!settings.general.siteName || settings.general.siteName.trim().length === 0) {
      this.toastService.error('O nome do site é obrigatório');
      return false;
    }

    // Validate security settings
    if (settings.security.sessionTimeout < 5 || settings.security.sessionTimeout > 480) {
      this.toastService.error('O timeout de sessão deve estar entre 5 e 480 minutos');
      return false;
    }

    if (settings.security.passwordExpiry < 30 || settings.security.passwordExpiry > 365) {
      this.toastService.error('A expiração de senha deve estar entre 30 e 365 dias');
      return false;
    }

    // Validate payment settings
    if (settings.payments.taxRate < 0 || settings.payments.taxRate > 100) {
      this.toastService.error('A taxa de IVA deve estar entre 0 e 100%');
      return false;
    }

    // Validate personalization settings
    if (settings.personalization.itemsPerPage < 10 || settings.personalization.itemsPerPage > 100) {
      this.toastService.error('Os itens por página devem estar entre 10 e 100');
      return false;
    }

    return true;
  }
}

