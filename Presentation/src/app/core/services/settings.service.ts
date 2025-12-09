import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ToastService } from './toast.service';

export interface AppSettings {
  general: {
    siteName: string;
    siteDescription: string;
    maintenanceMode: boolean;
  };
  security: {
    twoFactorAuth: boolean;
    sessionTimeout: number;
    passwordExpiry: number;
  };
  notifications: {
    emailNotifications: boolean;
    smsNotifications: boolean;
    pushNotifications: boolean;
  };
  payments: {
    currency: string;
    taxRate: number;
    paymentMethods: string[];
  };
  personalization: {
    theme: string;
    language: string;
    itemsPerPage: number;
  };
}

const DEFAULT_SETTINGS: AppSettings = {
  general: {
    siteName: 'DreamLuso',
    siteDescription: 'Plataforma de imóveis em Portugal',
    maintenanceMode: false
  },
  security: {
    twoFactorAuth: false,
    sessionTimeout: 30,
    passwordExpiry: 90
  },
  notifications: {
    emailNotifications: true,
    smsNotifications: false,
    pushNotifications: true
  },
  payments: {
    currency: 'EUR',
    taxRate: 23,
    paymentMethods: ['credit_card', 'mbway', 'bank_transfer']
  },
  personalization: {
    theme: 'light',
    language: 'pt',
    itemsPerPage: 20
  }
};

const STORAGE_KEY = 'dreamluso_admin_settings';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private readonly apiUrl = `${environment.apiUrl}/admin/settings`;
  private _settings = new BehaviorSubject<AppSettings>(DEFAULT_SETTINGS);
  public readonly settings$ = this._settings.asObservable();

  constructor(
    private http: HttpClient,
    private toastService: ToastService
  ) {
    this.loadSettings();
  }

  /**
   * Load settings from localStorage (fallback) or API
   */
  loadSettings(): void {
    // Try to load from localStorage first (for now)
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored) {
      try {
        const settings = JSON.parse(stored);
        this._settings.next({ ...DEFAULT_SETTINGS, ...settings });
        return;
      } catch (error) {
        console.error('Error parsing stored settings:', error);
      }
    }

    // If no stored settings, use defaults
    this._settings.next(DEFAULT_SETTINGS);
  }

  /**
   * Get current settings synchronously
   */
  getSettings(): AppSettings {
    return this._settings.value;
  }

  /**
   * Save settings to localStorage (and optionally to API)
   */
  saveSettings(settings: AppSettings): Observable<boolean> {
    // Save to localStorage immediately (for persistence)
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
    } catch (error) {
      console.error('Error saving to localStorage:', error);
      this.toastService.error('Erro ao salvar configurações localmente');
      return of(false);
    }

    // Try to save to API (if endpoint exists)
    // For now, we'll just save locally and show success
    // TODO: When backend endpoint is ready, uncomment this:
    /*
    return this.http.put<{ success: boolean }>(this.apiUrl, settings).pipe(
      tap(() => {
        this._settings.next(settings);
        this.toastService.success('Configurações salvas com sucesso!');
      }),
      catchError((error) => {
        console.error('Error saving settings to API:', error);
        // Settings are still saved locally, so we can show a warning
        this.toastService.warning('Configurações salvas localmente. Erro ao sincronizar com servidor.');
        return of(true); // Return true because local save succeeded
      })
    );
    */

    // For now, just update the BehaviorSubject and return success
    this._settings.next(settings);
    this.toastService.success('Configurações salvas com sucesso!');
    return of(true);
  }

  /**
   * Reset settings to defaults
   */
  resetSettings(): void {
    localStorage.removeItem(STORAGE_KEY);
    this._settings.next(DEFAULT_SETTINGS);
    this.toastService.info('Configurações restauradas para os valores padrão');
  }

  /**
   * Check if there are unsaved changes
   */
  hasUnsavedChanges(currentSettings: AppSettings): boolean {
    const saved = this.getSettings();
    return JSON.stringify(currentSettings) !== JSON.stringify(saved);
  }
}

