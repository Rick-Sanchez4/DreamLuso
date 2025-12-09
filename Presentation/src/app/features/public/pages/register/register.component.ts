import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { RegisterRequest, UserRole } from '../../../../core/models/user.model';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  // Expose UserRole enum to template
  UserRole = UserRole;
  
  userData: RegisterRequest = {
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    phone: '',
    role: UserRole.Client
  };
  confirmPassword: string = '';
  loading: boolean = false;

  constructor(
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.validateForm()) return;

    this.loading = true;
    
    // Prepare request data - remove phone if empty
    // Ensure role is sent as string (PascalCase) for backend enum deserialization
    const requestData: RegisterRequest = {
      firstName: this.userData.firstName.trim(),
      lastName: this.userData.lastName.trim(),
      email: this.userData.email.trim().toLowerCase(),
      password: this.userData.password,
      role: this.userData.role, // Will be serialized as "RealEstateAgent", "Client", or "Admin"
      ...(this.userData.phone?.trim() ? { phone: this.userData.phone.trim() } : {})
    };
    
    console.log('Current userData.role:', this.userData.role);
    console.log('UserRole enum values:', UserRole);
    console.log('Registering with data:', { ...requestData, password: '***' });
    console.log('Role type:', typeof requestData.role, 'Value:', requestData.role);
    
    this.authService.register(requestData).subscribe({
      next: (result: any) => {
        this.loading = false;
        
        if (result.isSuccess) {
          this.toastService.success('Conta criada com sucesso! Faça login para continuar.');
          this.router.navigate(['/login']);
        } else {
          // Handle validation errors
          let errorMessage = 'Erro ao criar conta';
          if (result.error) {
            if (result.error.errors) {
              // FluentValidation errors
              const firstError = Object.values(result.error.errors)[0] as string[];
              errorMessage = firstError?.[0] || result.error.message || errorMessage;
            } else if (result.error.description) {
              errorMessage = result.error.description;
            } else if (result.error.message) {
              errorMessage = result.error.message;
            }
          }
          this.toastService.error(errorMessage);
        }
      },
      error: (error: any) => {
        this.loading = false;
        console.error('Registration error:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', JSON.stringify(error.error, null, 2));
        console.error('Full error response:', error);
        
        // Handle validation errors from backend
        let errorMessage = 'Erro ao criar conta';
        if (error.error) {
          if (error.error.errors) {
            // FluentValidation errors - show all errors
            const validationErrors = error.error.errors;
            const errorMessages: string[] = [];
            Object.keys(validationErrors).forEach(key => {
              const errors = validationErrors[key] as string[];
              if (errors && errors.length > 0) {
                errorMessages.push(`${key}: ${errors.join(', ')}`);
              }
            });
            errorMessage = errorMessages.length > 0 ? errorMessages.join(' | ') : errorMessage;
          } else if (error.error.description) {
            errorMessage = error.error.description;
          } else if (error.error.message) {
            errorMessage = error.error.message;
          } else if (error.error.code) {
            // Handle specific error codes
            switch (error.error.code) {
              case 'EmailAlreadyExists':
                errorMessage = 'Este email já está registado. Tente fazer login ou use outro email.';
                break;
              case 'ValidationFailed':
                errorMessage = 'Os dados fornecidos são inválidos. Verifique os campos e tente novamente.';
                break;
              default:
                errorMessage = error.error.code + (error.error.description ? `: ${error.error.description}` : '');
            }
          } else {
            // Try to get any error message from the response
            errorMessage = JSON.stringify(error.error);
          }
        }
        this.toastService.error(errorMessage);
      }
    });
  }

  validateForm(): boolean {
    if (!this.userData.firstName || !this.userData.lastName) {
      this.toastService.warning('Preencha o nome completo');
      return false;
    }

    if (!this.userData.email) {
      this.toastService.warning('Preencha o email');
      return false;
    }

    if (!this.userData.password || this.userData.password.length < 6) {
      this.toastService.warning('A password deve ter pelo menos 6 caracteres');
      return false;
    }

    if (this.userData.password !== this.confirmPassword) {
      this.toastService.warning('As passwords não coincidem');
      return false;
    }

    return true;
  }
}

