import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AgentDashboardComponent } from './pages/dashboard/dashboard.component';
import { AgentPropertiesComponent } from './pages/properties/properties.component';
import { PropertyFormComponent } from './pages/property-form/property-form.component';
import { AgentProposalsComponent } from './pages/proposals/proposals.component';
import { AgentProfileComponent } from './pages/profile/profile.component';
import { AgentVisitsComponent } from './pages/visits/visits.component';
import { AgentContractsComponent } from './pages/contracts/contracts.component';

const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: AgentDashboardComponent },
  { path: 'properties/new', component: PropertyFormComponent },
  { path: 'properties/edit/:id', component: PropertyFormComponent },
  { path: 'properties', component: AgentPropertiesComponent },
  { path: 'proposals', component: AgentProposalsComponent },
  { path: 'visits', component: AgentVisitsComponent },
  { path: 'contracts', component: AgentContractsComponent },
  { path: 'profile', component: AgentProfileComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AgentRoutingModule { }
