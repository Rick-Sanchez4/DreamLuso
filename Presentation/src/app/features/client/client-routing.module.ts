import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientDashboardComponent } from './pages/dashboard/dashboard.component';
import { ClientProposalsComponent } from './pages/proposals/proposals.component';
import { ClientProfileComponent } from './pages/profile/profile.component';
import { ClientVisitsComponent } from './pages/visits/visits.component';
import { ClientProposalDetailComponent } from './pages/proposals/proposal-detail.component';
import { ClientFavoritesComponent } from './pages/favorites/favorites.component';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: ClientDashboardComponent },
      { path: 'proposals', component: ClientProposalsComponent },
      { path: 'proposals/:id', component: ClientProposalDetailComponent },
      { path: 'visits', component: ClientVisitsComponent },
      { path: 'favorites', component: ClientFavoritesComponent },
      { path: 'profile', component: ClientProfileComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }
