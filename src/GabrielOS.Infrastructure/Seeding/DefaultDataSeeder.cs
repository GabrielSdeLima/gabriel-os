using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GabrielOS.Infrastructure.Seeding;

public static class DefaultDataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Gabriel",
            Timezone = "America/Sao_Paulo",
            CurrentPhase = "Rebuilding",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(user);

        var defaultPillars = new[]
        {
            ("Body & Health", "Physical health, training, sleep, nutrition, body-mind integration"),
            ("Mind & Stability", "Mental health, emotional regulation, therapy, meditation, inner peace"),
            ("Career & Work", "Professional direction, positioning, skills, income, leadership"),
            ("Finances", "Cash flow, savings, investments, financial independence"),
            ("Relationships", "Family, friendships, romantic life, social connections"),
            ("Learning & Expansion", "Courses, certifications, languages, intellectual growth"),
            ("Freedom & Lifestyle", "Autonomy, living environment, daily structure, quality of life"),
        };

        for (int i = 0; i < defaultPillars.Length; i++)
        {
            var (name, description) = defaultPillars[i];
            await context.Pillars.AddAsync(new Pillar
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Name = name,
                Description = description,
                Priority = PillarPriority.Medium,
                Trend = Trend.Unknown,
                SortOrder = i,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
}
