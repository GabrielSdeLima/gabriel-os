using System.IO;
using System.Text;
using System.Text.Json;
using GabrielOS.Domain.Interfaces;
using GabrielOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GabrielOS.Application.Services;

public class ExportService
{
    private readonly AppDbContext _context;
    private readonly IJournalEntryRepository _journalRepo;

    public ExportService(AppDbContext context, IJournalEntryRepository journalRepo)
    {
        _context = context;
        _journalRepo = journalRepo;
    }

    public async Task<string> BackupDatabaseAsync(string targetDirectory)
    {
        var dbPath = _context.Database.GetDbConnection().ConnectionString
            .Replace("Data Source=", "", StringComparison.OrdinalIgnoreCase).Trim();

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var backupFile = Path.Combine(targetDirectory, $"gabrielos_backup_{timestamp}.db");

        if (File.Exists(dbPath))
            File.Copy(dbPath, backupFile, overwrite: true);

        return backupFile;
    }

    public async Task<string> ExportJournalMarkdownAsync(Guid userId, string targetDirectory)
    {
        var entries = await _journalRepo.GetByUserAsync(userId, take: 1000);
        var sb = new StringBuilder();
        sb.AppendLine("# Gabriel OS — Journal Export");
        sb.AppendLine($"_Exported: {DateTime.Now:dd MMM yyyy HH:mm}_");
        sb.AppendLine();

        foreach (var entry in entries)
        {
            sb.AppendLine($"## {entry.CreatedAt:dd MMM yyyy} — {entry.EntryType}");
            if (!string.IsNullOrWhiteSpace(entry.Title))
                sb.AppendLine($"**{entry.Title}**");
            sb.AppendLine();
            sb.AppendLine(entry.Content);
            sb.AppendLine();
            sb.AppendLine("---");
            sb.AppendLine();
        }

        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var file = Path.Combine(targetDirectory, $"journal_{timestamp}.md");
        await File.WriteAllTextAsync(file, sb.ToString());
        return file;
    }

    public async Task<string> ExportJsonAsync(Guid userId, string targetDirectory)
    {
        var pillars = await _context.Pillars.Where(p => p.UserId == userId).ToListAsync();
        var goals = await _context.Goals.Where(g => g.UserId == userId).ToListAsync();
        var decisions = await _context.Decisions.Where(d => d.UserId == userId).ToListAsync();
        var checkIns = await _context.CheckIns.Where(c => c.UserId == userId).ToListAsync();
        var journal = await _context.JournalEntries.Where(j => j.UserId == userId).ToListAsync();
        var reviews = await _context.WeeklyReviews.Where(r => r.UserId == userId).ToListAsync();
        var patterns = await _context.Patterns.Where(p => p.UserId == userId).ToListAsync();
        var metrics = await _context.Metrics.Where(m => m.UserId == userId).ToListAsync();
        var tasks = await _context.TaskItems.Where(t => t.UserId == userId).ToListAsync();

        var export = new
        {
            ExportedAt = DateTime.UtcNow,
            Pillars = pillars.Select(p => new { p.Id, p.Name, p.Score, p.Priority, p.Vision, p.CurrentState }),
            Goals = goals.Select(g => new { g.Id, g.Title, g.Status, g.Priority, g.HorizonType, g.NextAction }),
            Decisions = decisions.Select(d => new { d.Id, d.Title, d.Status, d.Context, d.ChosenOption, d.Rationale }),
            CheckIns = checkIns.Select(c => new { c.Date, c.Energy, c.Mood, c.Clarity, c.Tension, c.SuggestedMode }),
            Journal = journal.Select(j => new { j.CreatedAt, j.EntryType, j.Title, j.Content }),
            Reviews = reviews.Select(r => new { r.WeekStart, r.MainInsight, r.NextWeekFocus }),
            Patterns = patterns.Select(p => new { p.Name, p.Status, p.Description, p.Trigger }),
            Metrics = metrics.Select(m => new { m.Name, m.Value, m.Unit, m.Date }),
            Tasks = tasks.Select(t => new { t.Title, t.Status, t.Priority, t.IsNextAction })
        };

        var json = JsonSerializer.Serialize(export, new JsonSerializerOptions { WriteIndented = true });
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var file = Path.Combine(targetDirectory, $"gabrielos_export_{timestamp}.json");
        await File.WriteAllTextAsync(file, json);
        return file;
    }
}
