import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { LoginRequest } from '../../../../core/models/user.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  credentials: LoginRequest = {
    email: '',
    password: ''
  };
  loading: boolean = false;

  constructor(
    private authService: AuthService,
    private toastService: ToastService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.credentials.email || !this.credentials.password) {
      this.toastService.warning('Preencha todos os campos');
      return;
    }

    this.loading = true;
    this.authService.login(this.credentials).subscribe({
      next: (result: any) => {
        this.loading = false;
        
        if (result.isSuccess) {
          this.toastService.success('Login realizado com sucesso!');
          
          // Redirect based on role
          const user = this.authService.getCurrentUser();
          if (user) {
            switch (user.role) {
              case 'Admin':
                this.router.navigate(['/admin/dashboard']);
                break;
              case 'RealEstateAgent':
                this.router.navigate(['/agent/dashboard']);
                break;
              case 'Client':
                this.router.navigate(['/client/dashboard']);
                break;
              default:
                this.router.navigate(['/']);
            }
          } else {
            this.router.navigate(['/']);
          }
        } else {
          // Handle specific error types
          const errorCode = result.error?.code;
          const errorMessage = result.error?.description || 'Erro ao fazer login';
          
          if (errorCode === 'InvalidCredentials' || errorMessage.toLowerCase().includes('credenciais')) {
            this.toastService.error('Credenciais inválidas. Verifique o email e a password e tente novamente.');
          } else if (errorCode === 'UserInactive' || errorMessage.toLowerCase().includes('inativa') || errorMessage.toLowerCase().includes('aprovação')) {
            // Use the specific message from backend if available
            const inactiveMessage = errorMessage || 
              'A sua conta está inativa e aguarda aprovação do administrador. Você receberá uma notificação quando sua conta for aprovada.';
            this.toastService.error(inactiveMessage);
          } else {
            this.toastService.error(errorMessage);
          }
        }
      },
      error: (error: any) => {
        this.loading = false;
        console.error('Login error:', error);
        console.error('Error status:', error.status);
        console.error('Error details:', JSON.stringify(error.error, null, 2));
        console.error('Full error response:', error);
        
        // Handle HTTP errors (401, 400, etc.)
        if (error.status === 401 || error.status === 400) {
          let errorMessage = 'Credenciais inválidas';
          
          // Check for validation errors (FluentValidation format)
          if (error.error?.errors) {
            const validationErrors = error.error.errors;
            const errorMessages: string[] = [];
            Object.keys(validationErrors).forEach(key => {
              const errors = validationErrors[key] as string[];
              if (errors && errors.length > 0) {
                errorMessages.push(`${key}: ${errors.join(', ')}`);
              }
            });
            errorMessage = errorMessages.length > 0 ? errorMessages.join(' | ') : errorMessage;
          } else if (error.error?.description) {
            errorMessage = error.error.description;
          } else if (error.error?.message) {
            errorMessage = error.error.message;
          }
          
          // Check for specific error codes
          if (error.error?.code === 'InvalidCredentials') {
            this.toastService.error('Credenciais inválidas. Verifique o email e a password e tente novamente.');
          } else if (error.error?.code === 'UserInactive') {
            // Use the specific message from backend if available, otherwise use default
            const inactiveMessage = error.error?.description || 
              'A sua conta está inativa e aguarda aprovação do administrador. Você receberá uma notificação quando sua conta for aprovada.';
            this.toastService.error(inactiveMessage);
          } else {
            this.toastService.error(errorMessage);
          }
        } else {
          this.toastService.error('Erro ao conectar ao servidor. Tente novamente mais tarde.');
        }
      }
    });
  }
}
