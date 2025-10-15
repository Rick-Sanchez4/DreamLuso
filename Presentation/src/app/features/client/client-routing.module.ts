import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ClientDashboardComponent } from './pages/dashboard/dashboard.component';
import { ClientProposalsComponent } from './pages/proposals/proposals.component';
import { ClientProfileComponent } from './pages/profile/profile.component';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: ClientDashboardComponent },
      { path: 'proposals', component: ClientProposalsComponent },
      { path: 'profile', component: ClientProfileComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ClientRoutingModule { }
