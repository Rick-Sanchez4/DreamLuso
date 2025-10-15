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
    this.authService.login(this.credentials).subscribe(result => {
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
        this.toastService.error(result.error?.description || 'Erro ao fazer login');
      }
    });
  }
}
