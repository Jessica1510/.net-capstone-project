// courses.component.ts
import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';                   // still useful for other controls
import { HttpClient, HttpClientModule } from '@angular/common/http';

interface Course {
  id: number;
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  mode: string;
  cost: number;
  enrollments: any[];

}

// Matches your EnrollmentController GetUserEnrollments() shape
interface Enrollment {
  id: number;               // Course.Id
  title: string;
  description: string;
  startDate: string;
  endDate: string;
  mode: string;
  cost: number;
  enrolledAt: string;
}

@Component({
  selector: 'app-courses',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './courses.html',
  styleUrls: ['./courses.css']
})

export class CoursesComponent implements OnInit, OnDestroy {
  apiBase = 'https://localhost:7154/api'; // Adjust to your API base

  // UI state
  courses: Course[] = [];
  enrollments: Enrollment[] = [];

  loadingCourses = false;
  loadingEnrollments = false;
  signingUp: Record<number, boolean> = {}; // track sign-up per courseId

  errorCourses: string | null = null;
  errorEnrollments: string | null = null;
  infoMessage: string | null = null;

  // userId is sourced from localStorage
  userId: string | null = null;

  expanded: { [courseId: number]: boolean } = {};

  expandedEnrollments: { [enrollmentId: number]: boolean } = {};
  manageMode = false;


activeView: 'courses' | 'enrollments' = 'courses';

selectedCourse: Course | null = null;
isEditing = false;

courseForm: Course = {
  id: 0,
  title: '',
  description: '',
  startDate: '',
  endDate: '',
  mode: '',
  cost: 0,
  enrollments: [],
 
};
private formatDate(dateString: string): string {
  const date = new Date(dateString);
  return date.toISOString().split('T')[0]; // returns "yyyy-MM-dd"
}
setView(view: 'courses' | 'enrollments'): void {
  this.activeView = view;
}
toggleExpand(courseId: number): void {
  this.expanded[courseId] = !this.expanded[courseId];
}

  private onStorage = (e: StorageEvent) => {
    if (e.key === 'userId') {
      this.userId = this.getUserIdFromStorage();
      if (this.userId) {
        this.fetchEnrollments();
      } else {
        this.enrollments = [];
      }
    }
  };

  constructor(private http: HttpClient) {}

  ngOnInit(): void {
    this.userId = this.getUserIdFromStorage();
    this.fetchCourses();
    if (this.userId) {
      this.fetchEnrollments();
    }
    // React to login/logout from other tabs
    window.addEventListener('storage', this.onStorage);
  }
  
  ngOnDestroy(): void {
    window.removeEventListener('storage', this.onStorage);
  }

  isAlreadyEnrolled(courseId: number): boolean {
  return this.enrollments.some(e => e.id === courseId);
}
private getAuthHeaders(): { headers: any } {
  const token = localStorage.getItem('token');
  return {
    headers: {
      Authorization: `Bearer ${token}`
    }
  };
}
  private getUserIdFromStorage(): string | null {
    try {
      const v = localStorage.getItem('userId');
      return v && v.trim().length > 0 ? v.trim() : null;
    } catch {
      return null;
    }
  }

  // -------- Courses ----------
  fetchCourses(): void {
    this.loadingCourses = true;
    this.errorCourses = null;

    this.http.get<Course[]>(`${this.apiBase}/courses`).subscribe({
      next: (data) => {
        this.courses = data ?? [];
        this.loadingCourses = false;
      },
      error: (err) => {
        console.error('Failed to load courses', err);
        this.errorCourses = 'Failed to load courses. Please try again.';
        this.loadingCourses = false;
      }
    });
  }

  trackByCourseId(index: number, course: Course) {
    return course.id;
  }

  // -------- Enrollments ----------
  fetchEnrollments(): void {
    if (!this.userId) {
      this.errorEnrollments = 'Please sign in to view enrollments.';
      return;
    }
    this.loadingEnrollments = true;
    this.errorEnrollments = null;

    this.http.get<Enrollment[]>(`${this.apiBase}/enrollment/user/${encodeURIComponent(this.userId)}`).subscribe({
      next: (data) => {
        this.enrollments = data ?? [];
        this.loadingEnrollments = false;
      },
      error: (err) => {
        console.error('Failed to load enrollments', err);
        this.errorEnrollments = 'Failed to load your enrollments. Please try again.';
        this.loadingEnrollments = false;
      }
    });
  }

  // -------- Sign up ----------
  signUp(courseId: number): void {
    if (!this.userId) {
      this.infoMessage = null;
      this.errorEnrollments = 'Please sign in before signing up.';
      return;
    }

    this.signingUp[courseId] = true;
    this.infoMessage = null;
    this.errorEnrollments = null;

    const body = { userId: this.userId, courseId };

    // Your API returns plain text (e.g., "Enrollment successful.")
    this.http.post(`${this.apiBase}/enrollment/signup`, body, { responseType: 'text' }).subscribe({
      next: (msg) => {
        this.infoMessage = msg || 'Enrollment successful.';
        this.signingUp[courseId] = false;
        this.fetchEnrollments(); // refresh "My Enrollments"
      },
      error: (err) => {
        console.error('Sign up failed', err);
        const serverMsg = err?.error ? String(err.error) : 'Sign up failed. Please try again.';
        this.errorEnrollments = serverMsg;
        this.signingUp[courseId] = false;
      }
    });
  }

  // (Optional) Drop/un-enroll:
  drop(courseId: number): void {
    if (!this.userId) {
      this.errorEnrollments = 'Please sign in.';
      return;
    }
    this.loadingEnrollments = true;
    this.http.delete(`${this.apiBase}/enrollment/drop`, {
      params: { userId: this.userId, courseId: String(courseId) },
      responseType: 'text'
    }).subscribe({
      next: () => {
        this.loadingEnrollments = false;
        this.fetchEnrollments();
      },
      error: (err) => {
        console.error('Drop failed', err);
        this.errorEnrollments = err?.error ? String(err.error) : 'Drop failed. Please try again.';
        this.loadingEnrollments = false;
      }
    });
  }

  isTruncatable(description: string): boolean {
  return description.length > 150;
}



toggleEnrollmentExpand(id: number): void {
  this.expandedEnrollments[id] = !this.expandedEnrollments[id];
}

isEnrollmentTruncatable(description: string): boolean {
  return description.length > 150;
}
isAdmin(): boolean {
  const roles = JSON.parse(localStorage.getItem('roles') || '[]');
  return roles.includes('Admin');
}

toggleManageMode(): void {
  this.manageMode = !this.manageMode;
}


//Admin controls 
openCreateForm(): void {
  this.isEditing = false;
  this.selectedCourse = null;
  this.courseForm = {
    id: 0,
    title: '',
    description: '',
    startDate: '',
    endDate: '',
    mode: '',
    cost: 0,
    enrollments:[]
  };
}

openEditForm(course: Course): void {
  this.isEditing = true;
  this.selectedCourse = course;
  this.courseForm = {
    ...course,
    startDate: this.formatDate(course.startDate),
    endDate: this.formatDate(course.endDate),
    enrollments: course.enrollments ?? []
  };
}
saveCourse(): void {
  const url = this.isEditing
    ? `${this.apiBase}/courses/${this.selectedCourse?.id}`
    : `${this.apiBase}/courses`;

  const method = this.isEditing ? 'put' : 'post';

  this.http[method](url, this.courseForm, this.getAuthHeaders()).subscribe({
    next: () => {
      this.fetchCourses();
      this.courseForm = {
        id: 0,
        title: '',
        description: '',
        startDate: '',
        endDate: '',
        mode: '',
        cost: 0,
        enrollments: []
      };
      this.selectedCourse = null;
      this.isEditing = false;
    },
    error: (err) => {
      console.error('Course save failed', err);
      this.errorCourses = 'Failed to save course. Please try again.';
    }
  });
}

deleteCourse(courseId: number): void {
  if (!confirm('Are you sure you want to delete this course?')) return;

  this.http.delete(`${this.apiBase}/courses/${courseId}`, this.getAuthHeaders()).subscribe({
    next: () => this.fetchCourses(),
    error: (err) => {
      console.error('Delete failed', err);
      this.errorCourses = 'Failed to delete course. Please try again.';
    }
  });
}

}
