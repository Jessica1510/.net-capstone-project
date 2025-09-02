import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { RouterModule } from '@angular/router'; 
import { filter } from 'rxjs/operators';
import { RouterOutlet } from '@angular/router';
import {Login} from './components/login/login';




@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, Login, RouterModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App {
  showNavbar = true;
  userName: string = ''; // Set this after login
  protected readonly title = signal('cwms-angular');

  roles: string[] = [];

constructor(private router: Router) {
  this.router.events
    .pipe(filter(event => event instanceof NavigationEnd))
    .subscribe((event: NavigationEnd) => {
      this.showNavbar = !event.urlAfterRedirects.includes('/login');
    });

  const storedName = localStorage.getItem('userName');
  if (storedName) {
    this.userName = storedName;
  }

  const storedRoles = localStorage.getItem('roles');
  if (storedRoles) {
    this.roles = JSON.parse(storedRoles);
  }
}

isAdmin(): boolean {
  return this.roles.includes('Admin');
}

  logout(): void {
    // Clear user session and redirect to login
    localStorage.clear();
    this.userName = '';
    this.router.navigate(['/login']);
  }
}
