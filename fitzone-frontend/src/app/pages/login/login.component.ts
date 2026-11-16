import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="container d-flex justify-content-center align-items-center" style="min-height: 85vh;">
      <div class="card bg-card text-white p-4 shadow" style="width: 100%; max-width: 400px;">
        <h3 class="text-center text-warning fw-bold mb-4">SIGN IN</h3>
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="mb-3">
            <label class="form-label text-secondary">Email or Username</label>
            <input type="text" class="form-control bg-dark text-white border-secondary" formControlName="userName" placeholder="Enter email or username">
          </div>
          <div class="mb-3">
            <label class="form-label text-secondary">Password</label>
            <input type="password" class="form-control bg-dark text-white border-secondary" formControlName="password" placeholder="Enter password">
          </div>
          <button type="submit" class="btn btn-warning w-100 fw-bold text-dark" [disabled]="form.invalid">Login</button>
        </form>
        <p class="text-center text-secondary mt-3 mb-0 small">
          Don't have an account? <a routerLink="/register" class="text-warning">Register</a>
        </p>
      </div>
    </div>
  `
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.group({
    userName: ['', Validators.required],
    password: ['', Validators.required]
  });

  onSubmit() {
    if (this.form.invalid) return;
    this.auth.login(this.form.value as any).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => alert(err.error?.message || 'Invalid credentials')
    });
  }
}