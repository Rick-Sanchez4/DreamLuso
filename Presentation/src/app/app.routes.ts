import { Routes } from '@angular/router';
import { LandingComponent } from './features/public/pages/landing/landing.component';
import { PropertiesComponent } from './features/public/pages/properties/properties.component';
import { PropertyDetailComponent } from './features/public/pages/property-detail/property-detail.component';
import { LoginComponent } from './features/public/pages/login/login.component';
import { RegisterComponent } from './features/public/pages/register/register.component';

export const routes: Routes = [
  // Public Routes
  { path: '', component: LandingComponent },
  { path: 'properties', component: PropertiesComponent },
  { path: 'property/:id', component: PropertyDetailComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },

  // Client Routes (Lazy Loaded)
  {
    path: 'client',
    loadChildren: () => import('./features/client/client.module').then(m => m.ClientModule)
  },

  // Agent Routes (Lazy Loaded)
  {
    path: 'agent',
    loadChildren: () => import('./features/agent/agent.module').then(m => m.AgentModule)
  },

  // Admin Routes (Lazy Loaded)
  {
    path: 'admin',
    loadChildren: () => import('./features/admin/admin.module').then(m => m.AdminModule)
  },

  // Fallback
  { path: '**', redirectTo: '' }
];
