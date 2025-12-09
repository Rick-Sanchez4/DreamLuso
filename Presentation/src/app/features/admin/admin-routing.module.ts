import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminDashboardComponent } from './pages/dashboard/dashboard.component';
import { AdminUsersComponent } from './pages/users/users.component';
import { AdminPropertiesComponent } from './pages/properties/properties.component';
import { AdminSettingsComponent } from './pages/settings/settings.component';
import { AdminClientsComponent } from './pages/clients/clients.component';
import { AdminAgentsComponent } from './pages/agents/agents.component';
import { AdminProposalsComponent } from './pages/proposals/proposals.component';
import { AdminCommentsComponent } from './pages/comments/comments.component';
import { AdminAnalyticsComponent } from './pages/analytics/analytics.component';
import { AuthGuard } from '../../core/guards/auth.guard';
import { RoleGuard } from '../../core/guards/role.guard';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['Admin'] },
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AdminDashboardComponent },
      { path: 'users', component: AdminUsersComponent },
      { path: 'clients', component: AdminClientsComponent },
      { path: 'agents', component: AdminAgentsComponent },
      { path: 'properties', component: AdminPropertiesComponent },
      { path: 'proposals', component: AdminProposalsComponent },
      { path: 'comments', component: AdminCommentsComponent },
      { path: 'analytics', component: AdminAnalyticsComponent },
      { path: 'settings', component: AdminSettingsComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
