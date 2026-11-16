import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponseDto, LoginDto, RegisterDto, UpdateUsernameDto, ChangePasswordDto } from '../models/fit-zone.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);
  private apiUrl = 'https://localhost:7071/api/Auth';

  currentUser = signal<AuthResponseDto | null>(this.getStoredUser());

  private getStoredUser(): AuthResponseDto | null {
    const userJson = localStorage.getItem('fit_zone_user');
    return userJson ? JSON.parse(userJson) : null;
  }

  login(dto: LoginDto): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>(`${this.apiUrl}/login`, dto).pipe( tap(res => this.handleAuthSuccess(res)) ); }  register(dto: RegisterDto): Observable<AuthResponseDto> { return this.http.post<AuthResponseDto>(`${this.apiUrl}/register`, dto).pipe(
      tap(res => this.handleAuthSuccess(res))
    );
  }

  logout() {
    localStorage.removeItem('fit_zone_user');
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  updateUsername(dto: UpdateUsernameDto): Observable<any> {
    return this.http.put(`${this.apiUrl}/update-username`, dto).pipe( tap(() => { const user = this.currentUser(); if (user) { const updated = { ...user, userName: dto.newUserName }; this.handleAuthSuccess(updated); } }) ); }  changePassword(dto: ChangePasswordDto): Observable<any> { return this.http.post(`${this.apiUrl}/change-password`, dto);
  }

  assignRole(userId: number, role: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/assign-role`, { userId, role }); }  removeRole(userId: number, role: string): Observable<any> { return this.http.post(`${this.apiUrl}/remove-role`, { userId, role });
  }

  private handleAuthSuccess(res: AuthResponseDto) {
    localStorage.setItem('fit_zone_user', JSON.stringify(res));
    this.currentUser.set(res);
  }

  getToken(): string | null {
    return this.currentUser()?.token || null;
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  hasRole(role: string): boolean {
    return this.currentUser()?.roles?.includes(role) ?? false;
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(r => this.hasRole(r));
  }
}