import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CourseClaim {
  id: number;
  title: string;
}

@Injectable({
  providedIn: 'root'
})
export class Courseforclaims {
  private apiUrl = 'https://localhost:7154/api/courses';

  constructor(private http: HttpClient) {}

  getCourses(): Observable<CourseClaim[]> {
    return this.http.get<CourseClaim[]>(this.apiUrl);
  }
}