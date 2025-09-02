import { Routes } from '@angular/router';
import { Login } from './components/login/login';   
import { Home } from './components/home/home';
import { CoursesComponent } from './components/courses/courses';
import { Claims } from './components/claims/claims';
import { Certifications } from './components/certifications/certifications';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },

  // Protected routes
  { path: 'home', component: Home, canActivate: [AuthGuard] },
  { path: 'claims', component: Claims, canActivate: [AuthGuard] },
  { path: 'courses', loadComponent: () => import('./components/courses/courses').then(m => m.CoursesComponent), canActivate: [AuthGuard] },
  { path: 'certifications', component: Certifications, canActivate: [AuthGuard] },
  { path: '**', redirectTo: 'login' },

];

