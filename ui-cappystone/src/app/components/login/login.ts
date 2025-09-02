import { Component, ViewEncapsulation  } from '@angular/core';
import { FormBuilder, FormsModule } from '@angular/forms';
import { CommonModule } from "@angular/common";
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Router } from '@angular/router';
import { WorkerProfile } from '../../models/worker-profile';
import { FormGroup, FormControl, Validators,ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-login',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
  encapsulation: ViewEncapsulation.None
})
export class Login {
  loginForm = new FormGroup({
    username: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required)
  });
  

  constructor(private http: HttpClient, private router: Router, private fb: FormBuilder) {}

  onLogin() {
    const formData = this.loginForm.value;
    const body = {
      email: formData.username,
      password: formData.password
    };

    this.http.post<any>('https://localhost:7186/api/accounts/login', body)
      .subscribe({
        next: res => {
          console.log('Login success:', res);
          localStorage.setItem('token', res.token);
          localStorage.setItem('userId', res.userId);
          localStorage.setItem('roles', JSON.stringify(res.roles));

          const headers = new HttpHeaders({
            Authorization: `Bearer ${res.token}`
          });

          this.http.get<WorkerProfile>('https://localhost:7044/api/workers/me', { headers })
  .subscribe({
    next: user => {
      localStorage.setItem('workerProfile', JSON.stringify(user)); // ✅ Store profile
      this.router.navigate(['/home'], { state: { profile: user } });
    },
    error: err => {
      console.error('Failed to fetch user profile', err);
      const fallbackProfile: WorkerProfile = {
        fullName: 'User',
        email: '',
        phone: '',
        skillLevel: '',
        isActive: false,
        id: 0,
        userId: ''
      };
      localStorage.setItem('workerProfile', JSON.stringify(fallbackProfile)); // ✅ Store fallback
      this.router.navigate(['/home'], { state: { profile: fallbackProfile } });
    }
  });

        },
        error: err => {
          console.error('Login failed', err);
          alert('Invalid credentials');
        }
      });
  }

  onRegister() {
    this.router.navigate(['/register']);
  }
}
