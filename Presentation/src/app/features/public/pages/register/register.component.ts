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
    this.authService.register(this.userData).subscribe(result => {
      this.loading = false;
      
      if (result.isSuccess) {
        this.toastService.success('Conta criada com sucesso! Faça login para continuar.');
        this.router.navigate(['/login']);
      } else {
        this.toastService.error(result.error?.description || 'Erro ao criar conta');
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

