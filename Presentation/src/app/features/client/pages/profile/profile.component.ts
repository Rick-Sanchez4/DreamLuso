import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { User, UpdateProfileRequest } from '../../../../core/models/user.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';
import { ImageCropperComponent } from '../../../../shared/components/image-cropper/image-cropper.component';

@Component({
  selector: 'app-client-profile',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ClientSidebarComponent, ImageCropperComponent],
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

  defaultProfileImageUrl: string = 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMjAwIiBoZWlnaHQ9IjIwMCIgdmlld0JveD0iMCAwIDIwMCAyMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIyMDAiIGhlaWdodD0iMjAwIiBmaWxsPSIjMTBCOTgxIi8+CjxjaXJjbGUgY3g9IjEwMCIgY3k9IjgwIiByPSIzNSIgZmlsbD0id2hpdGUiLz4KPHBhdGggZD0iTTYwIDE2MEw2MCAxNDBDNjAgMTI2Ljc0NSA3MC43NDUyIDExNiA4NCAxMTZMMTE2IDExNkMxMjkuMjU1IDExNiAxNDAgMTI2Ljc0NSAxNDAgMTQwTDE0MCAxNjBMNjAgMTYwWiIgZmlsbD0id2hpdGUiLz4KPC9zdmc+';
  showCropper: boolean = false;
  selectedImageFile: File | null = null;

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
    if (!this.currentUser) {
      this.toastService.error('Usuário não encontrado');
      return;
    }

    // Client-side validation
    if (!this.profileForm.firstName || this.profileForm.firstName.trim() === '') {
      this.toastService.error('Primeiro nome é obrigatório');
      return;
    }

    if (!this.profileForm.lastName || this.profileForm.lastName.trim() === '') {
      this.toastService.error('Apelido é obrigatório');
      return;
    }

    // Validate phone number format if provided
    if (this.profileForm.phoneNumber && this.profileForm.phoneNumber.trim() !== '') {
      const phoneRegex = /^[0-9]{9,15}$/;
      if (!phoneRegex.test(this.profileForm.phoneNumber.replace(/\s+/g, ''))) {
        this.toastService.error('Telefone inválido. Use apenas números (9 a 15 dígitos)');
        return;
      }
    }

    this.loading = true;
    
    // Clean phone number (remove spaces, dashes, etc.)
    let cleanPhoneNumber = this.profileForm.phoneNumber?.trim() || null;
    if (cleanPhoneNumber) {
      cleanPhoneNumber = cleanPhoneNumber.replace(/\s+/g, '').replace(/[-\/\(\)]/g, '');
      if (cleanPhoneNumber === '') {
        cleanPhoneNumber = null;
      }
    }

    const requestBody = {
      firstName: this.profileForm.firstName.trim(),
      lastName: this.profileForm.lastName.trim(),
      phoneNumber: cleanPhoneNumber,
      address: this.profileForm.address?.trim() || null,
      dateOfBirth: null // Can be added later if needed
    };

    console.log('Sending profile update request:', requestBody);

    this.http.put(`${environment.apiUrl}/accounts/profile`, requestBody).subscribe({
      next: () => {
        this.toastService.success('Perfil atualizado com sucesso!');
        this.editMode = false;
        this.loading = false;
        
        // Update local user data
        if (this.currentUser) {
          this.currentUser.firstName = this.profileForm.firstName;
          this.currentUser.lastName = this.profileForm.lastName;
          this.currentUser.phoneNumber = this.profileForm.phoneNumber;
          this.currentUser.address = this.profileForm.address;
          // Update in AuthService
          this.authService.updateCurrentUser(this.currentUser);
        }
      },
      error: (error: any) => {
        console.error('Error updating profile:', error);
        console.error('Error details:', JSON.stringify(error.error, null, 2));
        
        let errorMsg = 'Erro ao atualizar perfil';
        
        if (error.error) {
          // Handle FluentValidation errors
          if (error.error.errors) {
            const validationErrors = error.error.errors;
            const errorMessages: string[] = [];
            Object.keys(validationErrors).forEach(key => {
              const errors = validationErrors[key] as string[];
              if (errors && errors.length > 0) {
                // Format property name for better readability
                const propertyName = key === 'FirstName' ? 'Primeiro Nome' :
                                   key === 'LastName' ? 'Apelido' :
                                   key === 'PhoneNumber' ? 'Telefone' :
                                   key === 'DateOfBirth' ? 'Data de Nascimento' : key;
                errorMessages.push(`${propertyName}: ${errors.join(', ')}`);
              }
            });
            errorMsg = errorMessages.length > 0 ? errorMessages.join(' | ') : errorMsg;
          } else if (error.error.description) {
            errorMsg = error.error.description;
          } else if (error.error.message) {
            errorMsg = error.error.message;
          } else if (typeof error.error === 'string') {
            errorMsg = error.error;
          }
        }
        
        this.toastService.error(errorMsg);
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
      // Se já é uma URL completa, usar diretamente
      if (profileImageUrl.startsWith('http')) {
        return profileImageUrl;
      }
      // Se começa com /, é um caminho relativo ao wwwroot (arquivos estáticos)
      // Usar baseUrl (sem /api) para arquivos estáticos
      if (profileImageUrl.startsWith('/')) {
        const baseUrl = environment.baseUrl || environment.apiUrl.replace('/api', '');
        return `${baseUrl}${profileImageUrl}`;
      }
      // Caso contrário, assumir que é relativo a /images/profiles/
      const baseUrl = environment.baseUrl || environment.apiUrl.replace('/api', '');
      return `${baseUrl}/images/profiles/${profileImageUrl}`;
    }
    return this.defaultProfileImageUrl;
  }

  handleImageError(event: Event): void {
    const img = event.target as HTMLImageElement;
    // Se já tentou o placeholder, não tentar novamente para evitar loop
    if (img.src === this.defaultProfileImageUrl || img.src.includes('data:image/svg')) {
      return;
    }
    // Tentar carregar placeholder apenas uma vez
    img.src = this.defaultProfileImageUrl;
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

      // Abrir cropper
      this.selectedImageFile = file;
      this.showCropper = true;
    }
  }

  onImageCropped(croppedFile: File): void {
    this.showCropper = false;
    this.uploadProfileImage(croppedFile);
  }

  onCropperCancel(): void {
    this.showCropper = false;
    this.selectedImageFile = null;
  }

  uploadProfileImage(file: File): void {
    this.loading = true;
    const formData = new FormData();
    formData.append('file', file);

    this.http.post(`${environment.apiUrl}/accounts/upload-profile-image`, formData).subscribe({
      next: (response: any) => {
        this.toastService.success('Imagem de perfil atualizada com sucesso!');
        this.loading = false;
        
        // Atualizar URL da imagem no usuário atual
        if (this.currentUser && response) {
          const imageUrl = typeof response === 'string' ? response : response.imageUrl || response;
          this.currentUser.profileImageUrl = imageUrl;
          
          // Atualizar no localStorage
          localStorage.setItem('currentUser', JSON.stringify(this.currentUser));
          
          // Atualizar no AuthService para sincronizar globalmente
          this.authService.updateCurrentUser(this.currentUser);
        }
      },
      error: (error: any) => {
        console.error('Error uploading profile image:', error);
        this.loading = false;
        
        let errorMsg = 'Erro ao fazer upload da imagem';
        
        // Detectar se o backend não está disponível (ERR_CONNECTION_REFUSED)
        if (error.status === 0 || 
            error.status === undefined || 
            error.statusText === 'Unknown Error' ||
            error.message?.includes('ERR_CONNECTION_REFUSED') ||
            error.message?.includes('Failed to fetch')) {
          errorMsg = '⚠️ Servidor não está disponível. Por favor, verifique se o backend está rodando na porta 5149 e tente novamente.';
        } else if (error.status === 401 || error.status === 403) {
          errorMsg = 'Você não tem autorização para fazer upload de imagens. Por favor, faça login novamente.';
        } else if (error.error) {
          if (error.error.code === 'UnauthorizedAccess') {
            errorMsg = 'Você não tem autorização para fazer upload de imagens. Por favor, faça login novamente.';
          } else if (error.error.code === 'InvalidInput') {
            errorMsg = error.error.description || 'Arquivo inválido. Verifique o tipo e tamanho da imagem.';
          } else if (error.error.description) {
            errorMsg = error.error.description;
          } else if (error.error.message) {
            errorMsg = error.error.message;
          }
        } else if (error.message) {
          errorMsg = error.message;
        }
        
        this.toastService.error(errorMsg, 'Erro no Upload');
      }
    });
  }
}

