import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="container d-flex justify-content-center align-items-center" style="min-height: 85vh;">
      <div class="card bg-card text-white p-4 shadow" style="width: 100%; max-width: 400px;">
        <h3 class="text-center text-warning fw-bold mb-4">CREATE ACCOUNT</h3>
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="row">
            <div class="col-md-6 mb-3">
              <label class="form-label text-secondary">First Name</label>
              <input type="text" class="form-control bg-dark text-white border-secondary" formControlName="firstName" placeholder="First name">
            </div>
            <div class="col-md-6 mb-3">
              <label class="form-label text-secondary">Last Name</label>
              <input type="text" class="form-control bg-dark text-white border-secondary" formControlName="lastName" placeholder="Last name">
            </div>
          </div>
          <div class="mb-3">
            <label class="form-label text-secondary">Username</label>
            <input type="text" class="form-control bg-dark text-white border-secondary" formControlName="userName" placeholder="Choose a username">
          </div>
          <div class="mb-3">
            <label class="form-label text-secondary">Email</label>
            <input type="email" class="form-control bg-dark text-white border-secondary" formControlName="email" placeholder="Enter email">
          </div>
          <div class="mb-3">
            <label class="form-label text-secondary">Password</label>
            <input type="password" class="form-control bg-dark text-white border-secondary" formControlName="password" placeholder="Min. 6 characters">
          </div>
          <button type="submit" class="btn btn-warning w-100 fw-bold text-dark" [disabled]="form.invalid">Register</button>
        </form>
        <p class="text-center text-secondary mt-3 mb-0 small">
          Already have an account? <a routerLink="/login" class="text-warning">Login</a>
        </p>
      </div>
    </div>
  `
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    userName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit() {
    if (this.form.invalid) return;
    this.auth.register(this.form.value as any).subscribe({
      next: () => this.router.navigate(['/']),
      error: err => alert(err.error?.message || 'Registration failed')
    });
  }
}