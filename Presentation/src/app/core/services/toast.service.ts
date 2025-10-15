import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ToastService {
  constructor(private toastr: ToastrService) {}

  success(message: string, title: string = 'Sucesso'): void {
    this.toastr.success(message, title);
  }

  error(message: string, title: string = 'Erro'): void {
    this.toastr.error(message, title);
  }

  warning(message: string, title: string = 'Atenção'): void {
    this.toastr.warning(message, title);
  }

  info(message: string, title: string = 'Informação'): void {
    this.toastr.info(message, title);
  }
}

