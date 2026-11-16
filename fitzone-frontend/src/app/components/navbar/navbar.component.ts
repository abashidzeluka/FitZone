import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  template: `
    <nav class="navbar navbar-expand-lg navbar-dark bg-card border-bottom border-secondary sticky-top">
      <div class="container">
        <a class="navbar-brand fw-bold text-warning fs-3" routerLink="/">
          <i class="bi bi-lightning-charge-fill"></i> FITZONE
        </a>
        
        <div class="d-flex align-items-center gap-3">
          <a class="nav-link text-white" routerLink="/" routerLinkActive="text-warning" [routerLinkActiveOptions]="{exact: true}">Home</a>
          
          @if (auth.isLoggedIn()) {
            <a class="nav-link text-white" routerLink="/dashboard" routerLinkActive="text-warning">Dashboard</a>
            
            @if (auth.hasAnyRole(['Trainer', 'Admin', 'superAdmin'])) {
              <a class="nav-link text-warning fw-semibold" routerLink="/admin" routerLinkActive="text-decoration-underline">
                <i class="bi bi-shield-lock"></i> Admin Panel
              </a>
            }

            <span class="badge bg-secondary text-warning ms-2">{{ auth.currentUser()?.userName }}</span>
            <button class="btn btn-sm btn-outline-danger" (click)="auth.logout()">Logout</button>
          } @else {
            <a class="btn btn-sm btn-outline-warning" routerLink="/login">Login</a>
            <a class="btn btn-sm btn-warning text-dark fw-bold" routerLink="/register">Register</a>
          }
        </div>
      </div>
    </nav>
  `
})
export class NavbarComponent {
  auth = inject(AuthService);
}