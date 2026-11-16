export interface AuthResponseDto {
  userId: number;
  userName: string;
  email: string;
  token: string;
  roles: string[];
}

export interface LoginDto {
  userName: string;
  password: string;
}

export interface RegisterDto {
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  password: string;
  dateOfBirth?: string | null;
}

export interface UpdateUsernameDto {
  newUserName: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
}

export interface TrainerDto {
  id: number;
  fullName: string;
  specialization: string;
  profileImageUrl?: string | null;
}

export interface TrainingSessionDto {
  id: number;
  title: string;
  description: string;
  trainerId: number;
  trainerName: string;
  startsAt: string;
  endsAt: string;
  capacity: number;
  availableSpots: number;
}

export interface CreateTrainingSessionDto {
  title: string;
  description: string;
  trainerId: number;
  startsAt: string;
  endsAt: string;
  capacity: number;
}

export interface CreateBookingDto {
  trainingSessionId: number;
}

export interface UserBookingDto {
  bookingId: number;
  userId: number;
  userName: string;
  trainingSessionId: number;
  sessionTitle: string;
  trainerName: string;
  startsAt: string;
  endsAt: string;
  status: string;
}

export interface MembershipPlanDto {
  id: number;
  name: string;
  visitLimit?: number | null;
  price: number;
  durationDays: number;
}

export interface UserMembershipDto {
  id: number;
  userId: number;
  planId: number;
  planName: string;
  startDate: string;
  endDate: string;
  remainingVisits?: number | null;
  isActive: boolean;
}

export interface MemberDto {
  id: number;
  userName: string;
  email: string;
  roles: string[];
}