using FitZone.Domain.Entities;
using FitZone.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FitZone.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<FitZoneDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }

            foreach (var role in Enum.GetNames<Role>())
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }

            var oldSuperAdmins = await userManager.Users
                .Where(u => u.UserName == "superadmin")
                .ToListAsync();
            foreach (var oldUser in oldSuperAdmins)
            {
                if (oldUser.UserName != "superAdmin")
                {
                    await userManager.DeleteAsync(oldUser);
                }
            }

            await EnsureUserAsync(userManager, "superAdmin", "superadmin@fitzone.ge", "Super", "Admin", "Admin123", Role.superAdmin.ToString());
            await EnsureUserAsync(userManager, "admin", "admin@fitzone.ge", "Nika", "Adminidze", "Pass123", Role.Admin.ToString());
            var trainerUser1 = await EnsureUserAsync(userManager, "giorgi.trainer", "giorgi@fitzone.ge", "Giorgi", "Beridze", "Pass123", Role.Trainer.ToString());
            var trainerUser2 = await EnsureUserAsync(userManager, "nino.trainer", "nino@fitzone.ge", "Nino", "Kapanadze", "Pass123", Role.Trainer.ToString());
            var memberUser1 = await EnsureUserAsync(userManager, "member1", "member1@fitzone.ge", "Luka", "Maisuradze", "Pass123", Role.Member.ToString());
            var memberUser2 = await EnsureUserAsync(userManager, "member2", "member2@fitzone.ge", "Mariam", "Gelashvili", "Pass123", Role.Member.ToString());

            if (!await context.Trainers.AnyAsync())
            {
                await context.Trainers.AddRangeAsync(
                    new Trainer
                    {
                        UserId = trainerUser1.Id,
                        FullName = $"{trainerUser1.FirstName} {trainerUser1.LastName}",
                        Specialization = "Strength & Conditioning"
                    },
                    new Trainer
                    {
                        UserId = trainerUser2.Id,
                        FullName = $"{trainerUser2.FirstName} {trainerUser2.LastName}",
                        Specialization = "HIIT & Functional Fitness"
                    });
                await context.SaveChangesAsync();
            }

            if (!await context.MembershipPlans.AnyAsync())
            {
                await context.MembershipPlans.AddRangeAsync(
                    new MembershipPlan { Name = "Basic 12 Visits", VisitLimit = 12, Price = 80m, DurationDays = 30 },
                    new MembershipPlan { Name = "Standard 20 Visits", VisitLimit = 20, Price = 120m, DurationDays = 30 },
                    new MembershipPlan { Name = "Unlimited Pro", VisitLimit = null, Price = 160m, DurationDays = 30 });
                await context.SaveChangesAsync();
            }

            if (!await context.UserMemberships.AnyAsync())
            {
                var unlimitedPlan = await context.MembershipPlans.FirstAsync(p => p.VisitLimit == null);
                var basicPlan = await context.MembershipPlans.FirstAsync(p => p.VisitLimit == 12);

                await context.UserMemberships.AddRangeAsync(
                    new UserMembership
                    {
                        UserId = memberUser1.Id,
                        PlanId = unlimitedPlan.Id,
                        StartDate = DateTime.UtcNow.AddDays(-5),
                        EndDate = DateTime.UtcNow.AddDays(25),
                        RemainingVisits = null
                    },
                    new UserMembership
                    {
                        UserId = memberUser2.Id,
                        PlanId = basicPlan.Id,
                        StartDate = DateTime.UtcNow.AddDays(-2),
                        EndDate = DateTime.UtcNow.AddDays(28),
                        RemainingVisits = 11
                    });
                await context.SaveChangesAsync();
            }

            if (!await context.TrainingSessions.AnyAsync())
            {
                var trainer1 = await context.Trainers.FirstAsync(t => t.UserId == trainerUser1.Id);
                var trainer2 = await context.Trainers.FirstAsync(t => t.UserId == trainerUser2.Id);
                var baseDate = DateTime.UtcNow.Date.AddDays(1);

                await context.TrainingSessions.AddRangeAsync(
                    new TrainingSession
                    {
                        Title = "Morning Full-Body Strength",
                        Description = "Compound lifts and progressive overload training for all fitness levels.",
                        TrainerId = trainer1.Id,
                        StartsAt = baseDate.AddHours(10),
                        EndsAt = baseDate.AddHours(11),
                        Capacity = 12
                    },
                    new TrainingSession
                    {
                        Title = "High-Intensity Interval Training (HIIT)",
                        Description = "45-minute high-paced metabolic conditioning and core workout.",
                        TrainerId = trainer2.Id,
                        StartsAt = baseDate.AddHours(18),
                        EndsAt = baseDate.AddHours(19),
                        Capacity = 15
                    },
                    new TrainingSession
                    {
                        Title = "Upper Body Hypertrophy",
                        Description = "Focused chest, back, and shoulder volume session.",
                        TrainerId = trainer1.Id,
                        StartsAt = baseDate.AddDays(1).AddHours(17),
                        EndsAt = baseDate.AddDays(1).AddHours(18).AddMinutes(30),
                        Capacity = 10
                    },
                    new TrainingSession
                    {
                        Title = "Functional Core & Mobility",
                        Description = "Dynamic stretching, joint mobility, and core stability exercises.",
                        TrainerId = trainer2.Id,
                        StartsAt = baseDate.AddDays(2).AddHours(11),
                        EndsAt = baseDate.AddDays(2).AddHours(12),
                        Capacity = 15
                    });
                await context.SaveChangesAsync();
            }

            if (!await context.Bookings.AnyAsync())
            {
                var firstSession = await context.TrainingSessions.OrderBy(s => s.Id).FirstAsync();
                var secondSession = await context.TrainingSessions.OrderBy(s => s.Id).Skip(1).FirstAsync();

                await context.Bookings.AddRangeAsync(
                    new Booking { UserId = memberUser1.Id, TrainingSessionId = firstSession.Id, Status = BookingStatus.Confirmed },
                    new Booking { UserId = memberUser2.Id, TrainingSessionId = secondSession.Id, Status = BookingStatus.Confirmed });
                await context.SaveChangesAsync();
            }
        }

        private static async Task<ApplicationUser> EnsureUserAsync(
            UserManager<ApplicationUser> userManager,
            string username,
            string email,
            string firstName,
            string lastName,
            string password,
            string role)
        {
            var user = await userManager.FindByNameAsync(username) ?? await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = new DateTime(1998, 5, 15),
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errs = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create user '{username}': {errs}");
                }
            }
            else if (user.UserName != username)
            {
                await userManager.SetUserNameAsync(user, username);
            }

            if (!await userManager.IsInRoleAsync(user, role))
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                if (currentRoles.Any())
                {
                    await userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                await userManager.AddToRoleAsync(user, role);
            }

            return user;
        }
    }
}