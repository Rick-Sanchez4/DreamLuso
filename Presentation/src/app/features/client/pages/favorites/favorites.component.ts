import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';
import { FavoriteService } from '../../../../core/services/favorite.service';
import { ClientService } from '../../../../core/services/client.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { Property } from '../../../../core/models/property.model';
import { PropertyCardComponent } from '../../../../shared/components/property-card/property-card.component';
import { User } from '../../../../core/models/user.model';

@Component({
  selector: 'app-client-favorites',
  standalone: true,
  imports: [CommonModule, RouterModule, ClientSidebarComponent, PropertyCardComponent],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss'
})
export class ClientFavoritesComponent implements OnInit {
  currentUser: User | null = null;
  clientId: string | null = null;
  favorites: Property[] = [];
  loading: boolean = true;

  constructor(
    private favoriteService: FavoriteService,
    private clientService: ClientService,
    private authService: AuthService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.currentUser = this.authService.getCurrentUser();
    if (this.currentUser) {
      this.loadClientProfile();
    } else {
      this.loading = false;
    }
  }

  loadClientProfile(): void {
    this.clientService.getByUserId(this.currentUser!.id).subscribe({
      next: (client: any) => {
        this.clientId = client?.id || null;
        if (this.clientId) {
          this.loadFavorites();
        } else {
          this.loading = false;
        }
      },
      error: (error) => {
        console.error('Error loading client profile:', error);
        this.loading = false;
      }
    });
  }

  loadFavorites(): void {
    if (!this.clientId) return;

    this.loading = true;
    this.favoriteService.getFavorites(this.clientId).subscribe({
      next: (result) => {
        if (result.isSuccess && result.value) {
          this.favorites = result.value;
        } else {
          this.favorites = [];
        }
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading favorites:', error);
        this.favorites = [];
        this.loading = false;
        this.toastService.error('Erro ao carregar favoritos');
      }
    });
  }

  removeFavorite(propertyId: string): void {
    if (!this.clientId) return;

    this.favoriteService.removeFavorite(this.clientId, propertyId).subscribe({
      next: (result) => {
        if (result.isSuccess) {
          this.toastService.success('Removido dos favoritos');
          this.loadFavorites(); // Reload list
        } else {
          this.toastService.error('Erro ao remover dos favoritos');
        }
      },
      error: (error) => {
        console.error('Error removing favorite:', error);
        this.toastService.error('Erro ao remover dos favoritos');
      }
    });
  }
}


