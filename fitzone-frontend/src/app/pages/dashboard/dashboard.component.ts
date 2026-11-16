import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FitZoneService } from '../../services/fit-zone.service';
import { AuthService } from '../../services/auth.service';
import { UserBookingDto, UserMembershipDto } from '../../models/fit-zone.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="container my-5 text-white">
      <h2 class="text-warning mb-4">My Dashboard</h2>

      <div class="row g-4">
        <div class="col-md-8">
          <!-- Active Memberships -->
          <div class="card bg-card p-4 mb-4">
            <h5 class="text-warning mb-3">My Memberships</h5>
            @for (m of memberships(); track m.id) {
              <div class="d-flex justify-content-between align-items-center border-bottom border-secondary py-2">
                <div>
                  <strong class="text-white">{{ m.planName }}</strong>
                  <div class="small text-secondary">
                    Valid: {{ m.startDate | date:'mediumDate' }} - {{ m.endDate | date:'mediumDate' }}
                    @if (m.remainingVisits !== null && m.remainingVisits !== undefined) {
                      <span> | Remaining visits: {{ m.remainingVisits }}</span>
                    }
                  </div>
                </div>
                <span class="badge" [class.bg-success]="m.isActive" [class.bg-secondary]="!m.isActive">
                  {{ m.isActive ? 'Active' : 'Expired' }}
                </span>
              </div>
            } @empty {
              <p class="text-secondary small mb-0">You have no active memberships.</p>
            }
          </div>

          <!-- My Bookings -->
          <div class="card bg-card p-4">
            <h5 class="text-warning mb-3">My Bookings</h5>
            <div class="table-responsive">
              <table class="table table-dark table-hover align-middle">
                <thead>
                  <tr>
                    <th>Session</th>
                    <th>Trainer</th>
                    <th>Date & Time</th>
                    <th>Status</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody>
                  @for (b of bookings(); track b.bookingId) {
                    <tr>
                      <td>{{ b.sessionTitle }}</td>
                      <td>{{ b.trainerName }}</td>
                      <td>{{ b.startsAt | date:'MMM d, y, h:mm a' }}</td>
                      <td>
                        <span class="badge" [class.bg-success]="b.status === 'Active'" [class.bg-danger]="b.status === 'Cancelled'">
                          {{ b.status }}
                        </span>
                      </td>
                      <td class="text-end">
                        @if (b.status === 'Active') {
                          <button class="btn btn-sm btn-outline-danger" (click)="cancelBooking(b.bookingId)">Cancel</button>
                        }
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <!-- Profile Settings -->
        <div class="col-md-4">
          <div class="card bg-card p-4 mb-4">
            <h5 class="text-warning mb-3">Update Username</h5>
            <form [formGroup]="usernameForm" (ngSubmit)="updateUsername()">
              <input type="text" class="form-control bg-dark text-white border-secondary mb-2" formControlName="newUserName" placeholder="New username">
              <button class="btn btn-warning text-dark fw-bold w-100 btn-sm">Update</button>
            </form>
          </div>

          <div class="card bg-card p-4">
            <h5 class="text-warning mb-3">Change Password</h5>
            <form [formGroup]="passwordForm" (ngSubmit)="changePassword()">
              <input type="password" class="form-control bg-dark text-white border-secondary mb-2" formControlName="currentPassword" placeholder="Current password">
              <input type="password" class="form-control bg-dark text-white border-secondary mb-2" formControlName="newPassword" placeholder="New password">
              <button class="btn btn-warning text-dark fw-bold w-100 btn-sm">Change Password</button>
            </form>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private fitService = inject(FitZoneService);
  private auth = inject(AuthService);
  private fb = inject(FormBuilder);

  bookings = signal<UserBookingDto[]>([]);
  memberships = signal<UserMembershipDto[]>([]);

  usernameForm = this.fb.group({ newUserName: ['', Validators.required] });
  passwordForm = this.fb.group({ currentPassword: ['', Validators.required], newPassword: ['', Validators.required] });

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.fitService.getMyBookings().subscribe(res => this.bookings.set(res));
    this.fitService.getMyMemberships().subscribe(res => this.memberships.set(res));
  }

  cancelBooking(bookingId: number) {
    this.fitService.cancelBooking(bookingId).subscribe(() => this.loadData());
  }

  updateUsername() {
    if (this.usernameForm.invalid) return;
    this.auth.updateUsername(this.usernameForm.value as any).subscribe({
      next: () => { alert('Username updated!'); this.usernameForm.reset(); },
      error: err => alert(err.error?.message || 'Error updating username')
    });
  }

  changePassword() {
    if (this.passwordForm.invalid) return;
    this.auth.changePassword(this.passwordForm.value as any).subscribe({
      next: () => { alert('Password changed!'); this.passwordForm.reset(); },
      error: err => alert(err.error?.message || 'Error changing password')
    });
  }
}