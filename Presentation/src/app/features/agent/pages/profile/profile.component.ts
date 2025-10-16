import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { AgentService, AgentProfile } from '../../../../core/services/agent.service';
import { ToastService } from '../../../../core/services/toast.service';
import { ThemeService } from '../../../../core/services/theme.service';
import { User, UpdateProfileRequest } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { AgentSidebarComponent } from '../../components/agent-sidebar/agent-sidebar.component';

@Component({
  selector: 'app-agent-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, AgentSidebarComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class AgentProfileComponent implements OnInit {
  currentUser: User | null = null;
  agentProfile: AgentProfile | null = null;
  loading: boolean = false;
  loadingProfile: boolean = true;
  editMode: boolean = false;
  
  profileForm: UpdateProfileRequest = {
    firstName: '',
    lastName: '',
    phoneNumber: '',
    address: ''
  };

  showPasswordSection: boolean = false;
  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  defaultProfileImageUrl: string = 'assets/images/default_avatar.jpeg';

  constructor(
    private authService: AuthService,
    private agentService: AgentService,
    private http: HttpClient,
    private toastService: ToastService,
    public themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadProfileData();
      this.loadAgentProfile();
    }
  }

  loadAgentProfile(): void {
    this.loadingProfile = true;
    // First load full user profile
    this.http.get<any>(`${environment.apiUrl}/accounts/profile`).subscribe({
      next: (userProfile) => {
        // Update currentUser with full data
        if (userProfile) {
          this.currentUser = {
            ...this.currentUser!,
            firstName: userProfile.firstName || this.currentUser!.firstName,
            lastName: userProfile.lastName || this.currentUser!.lastName,
            phoneNumber: userProfile.phoneNumber,
            address: userProfile.address,
            profileImageUrl: userProfile.profileImageUrl
          };
          this.loadProfileData();
        }
        
        // Then load agent-specific data
        this.agentService.getByUserId(this.currentUser!.id).subscribe({
          next: (agent: any) => {
            this.agentProfile = agent;
            this.loadingProfile = false;
          },
          error: (error) => {
            console.error('Error loading agent profile:', error);
            this.loadingProfile = false;
          }
        });
      },
      error: (error) => {
        console.error('Error loading user profile:', error);
        this.toastService.error('Erro ao carregar perfil');
        this.loadingProfile = false;
      }
    });
  }

  loadProfileData(): void {
    this.profileForm = {
      firstName: this.currentUser?.firstName || '',
      lastName: this.currentUser?.lastName || '',
      phoneNumber: this.currentUser?.phoneNumber || '',
      address: this.currentUser?.address || ''
    };
  }

  toggleEditMode(): void {
    if (this.editMode) {
      this.loadProfileData();
    }
    this.editMode = !this.editMode;
  }

  saveProfile(): void {
    this.loading = true;
    this.http.put(`${environment.apiUrl}/accounts/profile`, this.profileForm).subscribe({
      next: () => {
        this.toastService.success('Perfil atualizado com sucesso!');
        this.editMode = false;
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error(error.error?.message || 'Erro ao atualizar perfil');
        this.loading = false;
      }
    });
  }

  togglePasswordSection(): void {
    this.showPasswordSection = !this.showPasswordSection;
    if (!this.showPasswordSection) {
      this.resetPasswordForm();
    }
  }

  changePassword(): void {
    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.toastService.error('As passwords não coincidem');
      return;
    }

    this.loading = true;
    this.http.post(`${environment.apiUrl}/accounts/change-password`, {
      currentPassword: this.passwordForm.currentPassword,
      newPassword: this.passwordForm.newPassword
    }).subscribe({
      next: () => {
        this.toastService.success('Password alterada com sucesso!');
        this.resetPasswordForm();
        this.showPasswordSection = false;
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error(error.error?.message || 'Erro ao alterar password');
        this.loading = false;
      }
    });
  }

  resetPasswordForm(): void {
    this.passwordForm = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
  }

  getImageUrl(profileImageUrl: string | undefined): string {
    if (profileImageUrl && profileImageUrl.trim() !== '') {
      // If it's a full URL, use it directly
      if (profileImageUrl.startsWith('http')) {
        return profileImageUrl;
      }
      // Otherwise, prepend API URL
      return `${environment.apiUrl}/${profileImageUrl}`;
    }
    // Return a placeholder SVG data URL
    return 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgdmlld0JveD0iMCAwIDIwMCAyMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIyMDAiIGhlaWdodD0iMjAwIiBmaWxsPSIjMTBCOTgxIi8+CjxjaXJjbGUgY3g9IjEwMCIgY3k9IjgwIiByPSIzNSIgZmlsbD0id2hpdGUiLz4KPHBhdGggZD0iTTYwIDE2MEw2MCAxNDBDNjAgMTI2Ljc0NSA3MC43NDUyIDExNiA4NCAxMTZMMTE2IDExNkMxMjkuMjU1IDExNiAxNDAgMTI2Ljc0NSAxNDAgMTQwTDE0MCAxNjBMNjAgMTYwWiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+';
  }

  handleImageError(event: Event): void {
    (event.target as HTMLImageElement).src = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgdmlld0JveD0iMCAwIDIwMCAyMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIyMDAiIGhlaWdodD0iMjAwIiBmaWxsPSIjMTBCOTgxIi8+CjxjaXJjbGUgY3g9IjEwMCIgY3k9IjgwIiByPSIzNSIgZmlsbD0id2hpdGUiLz4KPHBhdGggZD0iTTYwIDE2MEw2MCAxNDBDNjAgMTI2Ljc0NSA3MC43NDUyIDExNiA4NCAxMTZMMTE2IDExNkMxMjkuMjU1IDExNiAxNDAgMTI2Ljc0NSAxNDAgMTQwTDE0MCAxNjBMNjAgMTYwWiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      if (!file.type.startsWith('image/')) {
        this.toastService.error('Por favor selecione uma imagem válida');
        return;
      }

      if (file.size > 5 * 1024 * 1024) {
        this.toastService.error('A imagem não pode exceder 5MB');
        return;
      }

      this.uploadProfileImage(file);
    }
  }

  uploadProfileImage(file: File): void {
    this.loading = true;
    const formData = new FormData();
    formData.append('file', file);

    this.http.post(`${environment.apiUrl}/accounts/upload-profile-image`, formData).subscribe({
      next: () => {
        this.toastService.success('Imagem de perfil atualizada!');
        this.loading = false;
      },
      error: (error) => {
        this.toastService.error(error.error?.message || 'Erro ao fazer upload da imagem');
        this.loading = false;
      }
    });
  }
}

