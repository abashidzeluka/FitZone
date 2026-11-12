# FitZone — Fitness Club Management Web API

FitZone is a backend RESTful Web API built with ASP.NET Core and Clean Architecture principles for managing a fitness club's operations, memberships, trainers, training sessions, and member bookings.

## Tech Stack

- **Framework:** ASP.NET Core Web API (.NET)
- **Database & ORM:** SQL Server, Entity Framework Core (Code-First)
- **Authentication & Authorization:** ASP.NET Core Identity, JWT Bearer Authentication, Role-Based Access Control (RBAC)
- **Testing:** xUnit, Moq, EF Core InMemory, `WebApplicationFactory` (Unit, Database & Integration Tests)
- **Documentation:** Swagger / OpenAPI

## Architecture

The solution is organized into distinct layers following Clean Architecture:

- **`FitZone.Domain`**: Core entities (`ApplicationUser`, `Trainer`, `MembershipPlan`, `UserMembership`, `TrainingSession`, `Booking`, `Visit`) and domain enums (`Role`, `BookingStatus`).
- **`FitZone.Application`**: Business logic services, DTOs, and validation rules.
- **`FitZone.Infrastructure`**: `FitZoneDbContext`, Entity Type Configurations, and database migrations.
- **`FitZone.API`**: REST controllers, JWT configuration, and global `ExceptionHandlingMiddleware`.
- **`FitZone.Tests`**: Automated test suite covering business services, database relationships, and HTTP endpoints.

## Key Features

- **Role Hierarchy & Access Control:**
  - `superAdmin`: Full system control, including assigning and revoking `Admin` roles.
  - `Admin`: Manages trainers, membership plans, training sessions, and views all bookings.
  - `Trainer`: Automatically synced with the `Trainers` table upon role assignment; manages own trainer profile.
  - `Member`: Purchases memberships, books and cancels own training sessions, and manages profile credentials.
- **Security & Data Integrity:**
  - IDOR protection on user memberships and session bookings.
  - Capacity enforcement, duplicate booking prevention, and active membership validation.
  - Centralized `ExceptionHandlingMiddleware` returning standardized JSON error responses (`400`, `401`, `403`, `404`, `500`).

## API Endpoints Overview

### Auth (`/api/Auth`)
- `POST /register` — Register a new member account
- `POST /login` — Authenticate and receive a JWT token
- `PUT /update-username` — Update username and refresh JWT token
- `PUT /change-password` — Change account password
- `POST /assign-role/{userId}/{role}` — Assign a role (`Admin` / `superAdmin`)
- `DELETE /remove-role/{userId}/{role}` — Revoke a role (`Admin` / `superAdmin`)

### Trainers (`/api/Trainers`)
- `GET /` — List all trainers
- `GET /{id}` — Get trainer details
- `PUT /{id}` — Update trainer profile (Owner Trainer or `Admin`)

### Membership Plans & User Memberships
- `GET /api/MembershipPlans` — View available plans
- `POST /api/UserMemberships` — Activate a membership plan for the authenticated user
- `GET /api/UserMemberships/my` — View active and past memberships

### Training Sessions & Bookings
- `GET /api/TrainingSessions` — List scheduled training sessions
- `POST /api/Bookings` — Book a training session
- `GET /api/Bookings/my` — View personal bookings
- `GET /api/Bookings` — View all bookings (`Admin` / `superAdmin`)
- `PATCH /api/Bookings/{id}/cancel` — Cancel a booking

## Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/FitZone.git
   cd FitZone
   ```

2. **Configure `appsettings.json` in `FitZone.API`:**
   Update the SQL Server connection string and JWT settings:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=.;Database=FitZoneDb;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "Jwt": {
       "Key": "YourSuperSecretKeyAtLeast32CharactersLong",
       "Issuer": "FitZoneAPI",
       "Audience": "FitZoneUsers"
     }
   }
   ```

3. **Apply database migrations:**
   ```bash
   dotnet ef database update --project FitZone.Infrastructure --startup-project FitZone.API
   ```

4. **Run the API:**
   ```bash
   dotnet run --project FitZone.API
   ```
   Open `https://localhost:<port>/swagger` to explore and test the endpoints.

## Running Tests

Execute the automated test suite (20 Unit, Database, and Integration tests):
```bash
dotnet test
```