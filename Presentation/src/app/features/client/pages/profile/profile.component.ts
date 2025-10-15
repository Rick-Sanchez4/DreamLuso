import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { User, UpdateProfileRequest } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';

@Component({
  selector: 'app-client-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ClientProfileComponent implements OnInit {
  currentUser: User | null = null;
  loading: boolean = false;
  editMode: boolean = false;
  
  // Profile form data
  profileForm: UpdateProfileRequest = {
    firstName: '',
    lastName: '',
    phoneNumber: '',
    address: ''
  };

  // Password change
  showPasswordSection: boolean = false;
  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  defaultProfileImageUrl: string = 'assets/images/default_avatar.jpeg';

  constructor(
    private authService: AuthService,
    private http: HttpClient,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadProfileData();
    }
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
      // Cancelling edit, reload original data
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
        
        // Refresh user data
        // In a real implementation, you'd refresh the user from the backend
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
    this.http.put(`${environment.apiUrl}/accounts/change-password`, {
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
    if (profileImageUrl) {
      return `${environment.apiUrl}/${profileImageUrl}`;
    }
    return this.defaultProfileImageUrl;
  }

  handleImageError(event: Event): void {
    (event.target as HTMLImageElement).src = this.defaultProfileImageUrl;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file type
      if (!file.type.startsWith('image/')) {
        this.toastService.error('Por favor selecione uma imagem válida');
        return;
      }

      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        this.toastService.error('A imagem não pode exceder 5MB');
        return;
      }

      // Upload image
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
        // Refresh user data
      },
      error: (error) => {
        this.toastService.error(error.error?.message || 'Erro ao fazer upload da imagem');
        this.loading = false;
      }
    });
  }
}

