import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FitZoneService } from '../../services/fit-zone.service';
import { AuthService } from '../../services/auth.service';
import { UserBookingDto, MemberDto, TrainerDto } from '../../models/fit-zone.models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container my-5 text-white">
      <h2 class="text-warning mb-4">Management Panel (Staff / Admin)</h2>

      <div class="row g-4">
        <!-- Create Session (Admin / SuperAdmin only) -->
        @if (auth.hasAnyRole(['Admin', 'superAdmin'])) {
          <div class="col-md-5">
            <div class="card bg-card p-4">
              <h5 class="text-warning mb-3">Create New Session</h5>
              <form [formGroup]="sessionForm" (ngSubmit)="createSession()">
                <div class="mb-2">
                  <label class="small text-secondary">Title</label>
                  <input type="text" class="form-control bg-dark text-white border-secondary" formControlName="title">
                </div>
                <div class="mb-2">
                  <label class="small text-secondary">Description</label>
                  <textarea class="form-control bg-dark text-white border-secondary" rows="2" formControlName="description"></textarea>
                </div>
                <div class="row">
                  <div class="col-6 mb-2">
                    <label class="small text-secondary">Start Time</label>
                    <input type="datetime-local" class="form-control bg-dark text-white border-secondary" formControlName="startsAt">
                  </div>
                  <div class="col-6 mb-2">
                    <label class="small text-secondary">End Time</label>
                    <input type="datetime-local" class="form-control bg-dark text-white border-secondary" formControlName="endsAt">
                  </div>
                </div>
                <div class="row">
                  <div class="col-6 mb-3">
                    <label class="small text-secondary">Capacity</label>
                    <input type="number" class="form-control bg-dark text-white border-secondary" formControlName="capacity">
                  </div>
                  <div class="col-6 mb-3">
                    <label class="small text-secondary">Trainer</label>
                    <select class="form-select bg-dark text-white border-secondary" formControlName="trainerId">
                      <option [ngValue]="null">Select...</option>
                      @for (t of trainers(); track t.id) {
                        <option [ngValue]="t.id">{{ t.fullName }}</option>
                      }
                    </select>
                  </div>
                </div>
                <button class="btn btn-warning text-dark fw-bold w-100" [disabled]="sessionForm.invalid">Create Session</button>
              </form>
            </div>
          </div>
        }

        <!-- All Bookings (Trainer, Admin, SuperAdmin) -->
        <div [class]="auth.hasAnyRole(['Admin', 'superAdmin']) ? 'col-md-7' : 'col-12'">
          <div class="card bg-card p-4">
            <h5 class="text-warning mb-3">All Bookings</h5>
            <div class="table-responsive" style="max-height: 400px;">
              <table class="table table-dark table-sm align-middle">
                <thead>
                  <tr>
                    <th>User</th>
                    <th>Session</th>
                    <th>Status</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  @for (b of allBookings(); track b.bookingId) {
                    <tr>
                      <td>{{ b.userName }}</td>
                      <td>{{ b.sessionTitle }}</td>
                      <td><span class="badge" [class.bg-success]="b.status === 'Active'" [class.bg-danger]="b.status === 'Cancelled'">{{ b.status }}</span></td>
                      <td>
                        @if (b.status === 'Active') {
                          <button class="btn btn-sm btn-outline-danger py-0" (click)="cancelBooking(b.bookingId)">Cancel</button>
                        }
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Members & Roles (Admin / SuperAdmin only) -->
        @if (auth.hasAnyRole(['Admin', 'superAdmin'])) {
          <div class="col-12">
            <div class="card bg-card p-4">
              <h5 class="text-warning mb-3">Members & Role Management</h5>
              <table class="table table-dark align-middle">
                <thead>
                  <tr>
                    <th>Username</th>
                    <th>Email</th>
                    <th>Roles</th>
                    <th>Assign Role</th>
                  </tr>
                </thead>
                <tbody>
                  @for (m of members(); track m.id) {
                    <tr>
                      <td>{{ m.userName }}</td>
                      <td>{{ m.email }}</td>
                      <td>
                        @for (r of m.roles; track r) {
                          <span class="badge bg-warning text-dark me-1">
                            {{ r }} 
                            <i class="bi bi-x-circle ms-1" style="cursor:pointer" (click)="removeRole(m.id, r)"></i>
                          </span>
                        }
                      </td>
                      <td>
                        <div class="btn-group btn-group-sm">
                          <button class="btn btn-outline-secondary text-white" (click)="assignRole(m.id, 'Trainer')">+ Trainer</button>
                          <button class="btn btn-outline-secondary text-white" (click)="assignRole(m.id, 'Admin')">+ Admin</button>
                        </div>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class AdminComponent implements OnInit {
  auth = inject(AuthService);
  private fitService = inject(FitZoneService);
  private fb = inject(FormBuilder);

  trainers = signal<TrainerDto[]>([]);
  allBookings = signal<UserBookingDto[]>([]);
  members = signal<MemberDto[]>([]);

  sessionForm = this.fb.group({
    title: ['', Validators.required],
    description: ['', Validators.required],
    startsAt: ['', Validators.required],
    endsAt: ['', Validators.required],
    capacity: [15, [Validators.required, Validators.min(1)]],
    trainerId: [null as number | null, Validators.required]
  });

  ngOnInit() {
    this.loadAdminData();
  }

  loadAdminData() {
    this.fitService.getAllBookings().subscribe(res => this.allBookings.set(res));
    if (this.auth.hasAnyRole(['Admin', 'superAdmin'])) {
      this.fitService.getTrainers().subscribe(res => this.trainers.set(res));
      this.fitService.getMembers().subscribe(res => this.members.set(res));
    }
  }

  createSession() {
    if (this.sessionForm.invalid) return;
    this.fitService.createSession(this.sessionForm.value as any).subscribe({
      next: () => {
        alert('Session created!');
        this.sessionForm.reset({ capacity: 15, trainerId: null });
      },
      error: err => alert(err.error?.message || 'Error creating session')
    });
  }

  cancelBooking(bookingId: number) {
    this.fitService.cancelBooking(bookingId).subscribe(() => this.loadAdminData());
  }

  assignRole(userId: number, role: string) {
    this.auth.assignRole(userId, role).subscribe(() => this.loadAdminData());
  }

  removeRole(userId: number, role: string) {
    this.auth.removeRole(userId, role).subscribe(() => this.loadAdminData());
  }
}