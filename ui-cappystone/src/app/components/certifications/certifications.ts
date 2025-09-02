import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpHeaders } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { Courseforclaims, CourseClaim } from '../../courseforclaims';

@Component({
  selector: 'app-certifications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './certifications.html',
  styleUrls: ['./certifications.css']
})
export class Certifications implements OnInit {
  courses: CourseClaim[] = [];
  certifications: any[] = [];
  selectedCert: any = null;

  showForm = false;

  newCert = {
    courseId: 1,
    issuedOn: '',
    validTill: '',
    credentialNumber: ''
  };

  constructor(private http: HttpClient, private router: Router, private coursesService: Courseforclaims) {}

  isExpired(cert: any): boolean {
    const today = new Date();
    return new Date(cert.validTill) < today;
  }
  /*
  ngOnInit(): void {
    this.http.get<any[]>('https://localhost:7188/api/Certification')
      .subscribe(data => {
        this.certifications = data;
      });
  }
  */

  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found');
      return;
    }

    this.coursesService.getCourses().subscribe({
      next: data => {
        this.courses = data;
      },
      error: err => {
        console.error('Failed to fetch courses:', err);
      }
    });
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    this.http.get<any[]>('https://localhost:7188/api/Certification/mine', { headers })
      .subscribe({
        next: data => {
          this.certifications = data;
        },
        error: err => {
          console.error('Failed to fetch certifications:', err);
        }
      });
  }  

  
  openDetails(cert: any): void {
    this.selectedCert = cert;
  }

  closeDetails(): void {
    this.selectedCert = null;
  }

  getCourseLabel(courseId: number): string {
  const course = this.courses.find(c => c.id === courseId);
  return course ? course.title : `Course #${courseId}`;
}

  submitCert(): void {
    const token = localStorage.getItem('token');
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    const payload = {
      courseId: this.newCert.courseId,
      credentialNumber: this.newCert.credentialNumber,
      issuedOn: new Date(this.newCert.issuedOn),
      validTill: new Date(this.newCert.validTill)
    };
  
    this.http.post<any>('https://localhost:7188/api/Certification/mine', payload, { headers })
      .subscribe({
        next: cert => {
          this.certifications.push(cert);
          this.showForm = false;
          this.newCert = {
            courseId: 1,
            issuedOn: '',
            validTill: '',
            credentialNumber: ''
          };
        },
        error: err => {
          console.error('Failed to add certification:', err);
        }
      });
  }  
  
  removeCert(id: number, event: Event): void {
    event.stopPropagation(); // prevent modal from opening
  
    const token = localStorage.getItem('token');
    if (!token) {
      console.error('No token found');
      return;
    }
  
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
  
    this.http.delete(`https://localhost:7188/api/Certification/${id}`, { headers })
      .subscribe({
        next: () => {
          this.certifications = this.certifications.filter(c => c.id !== id);
        },
        error: err => {
          console.error('Failed to delete certification:', err);
          alert('Unable to remove certification.');
        }
      });
  }  
}

/*
import { Component } from '@angular/core';

@Component({
  selector: 'app-certifications',
  imports: [],
  templateUrl: './certifications.html',
  styleUrl: './certifications.css'
})
export class Certifications {

}
*/
