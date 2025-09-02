import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from "@angular/common";
import { WorkerProfile } from '../../models/worker-profile';
@Component({
  selector: 'app-home',
  imports: [CommonModule],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home {
  worker: WorkerProfile;

  constructor(private router: Router) {
    const nav = this.router.getCurrentNavigation();
    const stateProfile = nav?.extras?.state?.['profile'];

    if (stateProfile) {
      this.worker = stateProfile;
      localStorage.setItem('workerProfile', JSON.stringify(stateProfile)); // Optional: refresh stored profile
    } else {
      const stored = localStorage.getItem('workerProfile');
      this.worker = stored ? JSON.parse(stored) : {
        fullName: 'User',
        email: '',
        phone: '',
        skillLevel: '',
        isActive: false,
        id: 0,
        userId: ''
      };
    }

    console.log('Loaded profile:', this.worker);
  }
  isAdmin(): boolean {
  const roles = JSON.parse(localStorage.getItem('roles') || '[]');
  return roles.includes('Admin');
}
}
