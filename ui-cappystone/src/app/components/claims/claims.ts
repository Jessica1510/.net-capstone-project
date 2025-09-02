import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { Courseforclaims, CourseClaim } from '../../courseforclaims';

@Component({
  selector: 'app-claims',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './claims.html',
  styleUrl: './claims.css'
})
export class Claims implements OnInit {
  editingClaimId: number | null = null;
  availableCourses: CourseClaim[] = [];
  claimForm: FormGroup;
  claims: any[] = [];
  showForm = false;
  workerId = localStorage.getItem('userId') || '';

  constructor(private fb: FormBuilder, private http: HttpClient, private router: Router, private courseService: Courseforclaims) {
    this.claimForm = this.fb.group({
      courseId: [''],
      amount: [''],
      remarks: ['']
    });
  }

  selectedStatus: string = '';
  filteredClaims: any[] = [];

  ngOnInit() {
    if (!this.workerId) {
      this.router.navigate(['/login']);
      return;
    }
    this.loadClaims();
    this.loadCourses();
  }
  loadCourses() {
    this.courseService.getCourses().subscribe(data => {
      this.availableCourses = data;
    });
  }
  getCourseTitle(courseId: number): string {
  const course = this.availableCourses.find(c => c.id === courseId);
  return course ? course.title : 'Unknown Course';
}
  loadClaims() {
    this.http.get<any[]>(`https://localhost:7153/api/claims/worker/${this.workerId}`)
      .subscribe(data => {
        this.claims = data;
        this.filterClaims();
      });
  }
  filterClaims() {
  if (!this.selectedStatus) {
    this.filteredClaims = this.claims;
  } else {
    this.filteredClaims = this.claims.filter(c => c.status === this.selectedStatus);
  }
}

  createClaim() {
    const claimData = {
      workerId: this.workerId,
      courseId: this.claimForm.value.courseId,
      amount: this.claimForm.value.amount,
      remarks: this.claimForm.value.remarks,
      submittedOn: new Date().toISOString(),
      status: 'Pending'
    };

    this.http.post(`https://localhost:7153/api/claims`, claimData)
      .subscribe(() => {
        this.resetForm();
        this.loadClaims();
      });
  }

  updateClaim() {
    if (!this.editingClaimId) return;

    const claimData = {
      id: this.editingClaimId,
      workerId: this.workerId,
      courseId: this.claimForm.value.courseId,
      amount: this.claimForm.value.amount,
      remarks: this.claimForm.value.remarks,
      status: 'Pending'
      // submittedOn is intentionally omitted to preserve original
    };

    this.http.put(`https://localhost:7153/api/claims/${this.editingClaimId}`, claimData)
      .subscribe(() => {
        this.resetForm();
        this.loadClaims();
      });
  }

  submitClaim() {
    if (this.editingClaimId) {
      this.updateClaim();
    } else {
      this.createClaim();
    }
  }

  editClaim(claim: any) {
    this.claimForm.patchValue({
      courseId: claim.courseId,
      amount: claim.amount,
      remarks: claim.remarks
    });

    this.editingClaimId = claim.id;
    this.showForm = true;
  }

  resetForm() {
    this.claimForm.reset();
    this.showForm = false;
    this.editingClaimId = null;
  }

  deleteClaim(claimId: number) {
  if (!confirm('Are you sure you want to delete this claim?')) return;

  this.http.delete(`https://localhost:7153/api/claims/${claimId}`)
    .subscribe(() => {
      this.loadClaims(); // Refresh the list
    }, error => {
      console.error('Delete failed', error);
    });
}
}
