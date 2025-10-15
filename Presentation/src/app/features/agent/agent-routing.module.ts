import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AgentDashboardComponent } from './pages/dashboard/dashboard.component';
import { AgentPropertiesComponent } from './pages/properties/properties.component';
import { AgentProposalsComponent } from './pages/proposals/proposals.component';
import { AgentProfileComponent } from './pages/profile/profile.component';

const routes: Routes = [
  {
    path: '',
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: AgentDashboardComponent },
      { path: 'properties', component: AgentPropertiesComponent },
      { path: 'proposals', component: AgentProposalsComponent },
      { path: 'profile', component: AgentProfileComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AgentRoutingModule { }
