import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FitZoneService } from '../../services/fit-zone.service';
import { AuthService } from '../../services/auth.service';
import { TrainingSessionDto, TrainerDto, MembershipPlanDto } from '../../models/fit-zone.models';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <!-- Hero -->
    <div class="py-5 text-center border-bottom border-secondary" style="background: linear-gradient(135deg, #121212 0%, #1f1f1f 100%);">
      <div class="container py-4">
        <h1 class="display-4 fw-bold text-warning">TRANSFORM YOUR BODY AT FITZONE</h1>
        <p class="lead text-secondary">Modern training facility, certified personal trainers, and flexible schedules.</p>
      </div>
    </div>

    <div class="container my-5">
      <!-- Membership Plans -->
      <h2 class="text-warning mb-4"><i class="bi bi-award-fill"></i> Membership Plans</h2>
      <div class="row g-4 mb-5">
        @for (plan of plans(); track plan.id) {
          <div class="col-md-4">
            <div class="card bg-card text-white h-100 shadow">
              <div class="card-body d-flex flex-column">
                <h4 class="text-warning fw-bold">{{ plan.name }}</h4>
                <p class="text-secondary">
                  {{ plan.visitLimit ? plan.visitLimit + ' visits included' : 'Unlimited visits' }}
                </p>
                <h3 class="mt-auto fw-bold">\${{ plan.price }} <small class="text-secondary fs-6">/ {{ plan.durationDays }} days</small></h3>
                <button class="btn btn-warning text-dark fw-bold mt-3" (click)="buyPlan(plan.id)">Choose Plan</button>
              </div>
            </div>
          </div>
        }
      </div>

      <!-- Training Sessions -->
      <h2 class="text-warning mb-4"><i class="bi bi-calendar2-check-fill"></i> Upcoming Training Sessions</h2>
      <div class="row g-4 mb-5">
        @for (session of sessions(); track session.id) {
          <div class="col-md-4">
            <div class="card bg-card text-white h-100 shadow">
              <div class="card-body d-flex flex-column">
                <div class="d-flex justify-content-between align-items-start mb-2">
                  <h5 class="text-warning fw-bold mb-0">{{ session.title }}</h5>
                  <span class="badge bg-dark border border-warning text-warning">
                    Spots: {{ session.availableSpots }}/{{ session.capacity }}
                  </span>
                </div>
                <p class="small text-secondary mb-2"><i class="bi bi-person-badge"></i> Trainer: {{ session.trainerName }}</p>
                <p class="small text-info mb-2"><i class="bi bi-clock"></i> {{ session.startsAt | date:'MMM d, y, h:mm a' }}</p>
                <p class="card-text small text-light">{{ session.description }}</p>
                
                <button class="btn btn-outline-warning mt-auto w-100 fw-bold"
                        [disabled]="session.availableSpots <= 0"
                        (click)="bookSession(session.id)">
                  {{ session.availableSpots > 0 ? 'Book Now' : 'Fully Booked' }}
                </button>
              </div>
            </div>
          </div>
        }
      </div>

      <!-- Trainers -->
      <h2 class="text-warning mb-4"><i class="bi bi-people-fill"></i> Our Trainers</h2>
      <div class="row g-4">
        @for (trainer of trainers(); track trainer.id) {
          <div class="col-md-3">
            <div class="card bg-card text-white h-100 text-center p-3">
              <img [src]="getTrainerImage(trainer.profileImageUrl)" 
                   class="rounded-circle mx-auto mb-3 border border-warning" 
                   style="width: 100px; height: 100px; object-fit: cover;">
              <h5 class="text-warning mb-1">{{ trainer.fullName }}</h5>
              <span class="badge bg-secondary mb-2">{{ trainer.specialization }}</span>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class HomeComponent implements OnInit {
  private fitService = inject(FitZoneService);
  private auth = inject(AuthService);
  private router = inject(Router);

  sessions = signal<TrainingSessionDto[]>([]);
  plans = signal<MembershipPlanDto[]>([]);
  trainers = signal<TrainerDto[]>([]);

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.fitService.getSessions().subscribe(res => this.sessions.set(res));
    this.fitService.getMembershipPlans().subscribe(res => this.plans.set(res));
    this.fitService.getTrainers().subscribe(res => this.trainers.set(res));
  }

  getTrainerImage(url?: string | null): string {
    if (!url) return 'https://via.placeholder.com/150/1a1a1a/ffc107?text=Trainer';
    return url.startsWith('http') ? url : `https://localhost:7071${url}`;
  }

  bookSession(sessionId: number) {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.fitService.createBooking({ trainingSessionId: sessionId }).subscribe({
      next: () => {
        alert('Session booked successfully!');
        this.loadData();
      },
      error: err => alert(err.error?.message || 'Booking failed')
    });
  }

  buyPlan(planId: number) {
    if (!this.auth.isLoggedIn()) {
      this.router.navigate(['/login']);
      return;
    }
    this.fitService.purchaseMembership(planId).subscribe({
      next: () => alert('Membership activated successfully!'),
      error: err => alert(err.error?.message || 'Error purchasing membership')
    });
  }
}