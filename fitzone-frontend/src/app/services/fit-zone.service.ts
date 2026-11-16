import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  UserBookingDto, CreateBookingDto, TrainingSessionDto,
  CreateTrainingSessionDto, TrainerDto, MembershipPlanDto,
  UserMembershipDto, MemberDto
} from '../models/fit-zone.models';

@Injectable({
  providedIn: 'root'
})
export class FitZoneService {
  private http = inject(HttpClient);
  private base = 'https://localhost:7071/api';

  // Sessions & Trainers
  getSessions(): Observable<TrainingSessionDto[]> {
    return this.http.get<TrainingSessionDto[]>(`${this.base}/TrainingSessions`);
  }

  createSession(dto: CreateTrainingSessionDto): Observable<TrainingSessionDto> {
    return this.http.post<TrainingSessionDto>(`${this.base}/TrainingSessions`, dto);
  }

  getTrainers(): Observable<TrainerDto[]> {
    return this.http.get<TrainerDto[]>(`${this.base}/Trainers`);
  }

  // Bookings
  getAllBookings(): Observable<UserBookingDto[]> {
    return this.http.get<UserBookingDto[]>(`${this.base}/Bookings`);
  }

  getMyBookings(): Observable<UserBookingDto[]> {
    return this.http.get<UserBookingDto[]>(`${this.base}/Bookings/my`);
  }

  createBooking(dto: CreateBookingDto): Observable<UserBookingDto> {
    return this.http.post<UserBookingDto>(`${this.base}/Bookings`, dto);
  }

  cancelBooking(bookingId: number): Observable<any> {
    return this.http.patch(`${this.base}/Bookings/${bookingId}/cancel`, {});
  }

  // Memberships & Members
  getMembershipPlans(): Observable<MembershipPlanDto[]> {
    return this.http.get<MembershipPlanDto[]>(`${this.base}/MembershipPlans`);
  }

  purchaseMembership(planId: number): Observable<UserMembershipDto> {
    return this.http.post<UserMembershipDto>(`${this.base}/UserMemberships`, { planId });
  }

  getMyMemberships(): Observable<UserMembershipDto[]> {
    return this.http.get<UserMembershipDto[]>(`${this.base}/UserMemberships/my`);
  }

  getMembers(): Observable<MemberDto[]> {
    return this.http.get<MemberDto[]>(`${this.base}/Members`);
  }
}