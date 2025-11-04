import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { ClientSidebarComponent } from '../../components/client-sidebar/client-sidebar.component';

@Component({
  selector: 'app-client-favorites',
  standalone: true,
  imports: [CommonModule, RouterModule, ClientSidebarComponent],
  templateUrl: './favorites.component.html',
  styleUrl: './favorites.component.scss'
})
export class ClientFavoritesComponent {
}


