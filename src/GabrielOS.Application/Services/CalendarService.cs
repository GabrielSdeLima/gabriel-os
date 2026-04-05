using GabrielOS.Domain.Entities;
using GabrielOS.Domain.Enums;
using GabrielOS.Domain.Interfaces;

namespace GabrielOS.Application.Services;

public class CalendarItem
{
    public Guid? SourceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CalendarItemSource Source { get; set; }
    public string? EventType { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }
    public bool IsAllDay { get; set; } = true;
    public bool IsCompleted { get; set; }
    public string? Location { get; set; }
    public GoalPriority? Priority { get; set; }
    public string? PillarName { get; set; }
    public string? GoalTitle { get; set; }
    public int? CheckInEnergy { get; set; }
}

public enum CalendarItemSource
{
    UserEvent,
    GoalDeadline,
    GoalReview,
    TaskDue,
    CheckIn
}

public class CalendarService
{
    private readonly ICalendarEventRepository _eventRepo;
    private readonly IGoalRepository _goalRepo;
    private readonly ITaskItemRepository _taskRepo;
    private readonly ICheckInRepository _checkInRepo;

    public CalendarService(
        ICalendarEventRepository eventRepo,
        IGoalRepository goalRepo,
        ITaskItemRepository taskRepo,
        ICheckInRepository checkInRepo)
    {
        _eventRepo = eventRepo;
        _goalRepo = goalRepo;
        _taskRepo = taskRepo;
        _checkInRepo = checkInRepo;
    }

    public Task<IReadOnlyList<CalendarEvent>> GetByUserAsync(Guid userId)
        => _eventRepo.GetByUserAsync(userId);

    public Task<IReadOnlyList<CalendarEvent>> GetByDateRangeAsync(Guid userId, DateTime start, DateTime end)
        => _eventRepo.GetByDateRangeAsync(userId, start, end);

    public Task<IReadOnlyList<CalendarEvent>> GetUpcomingAsync(Guid userId, int days = 7)
        => _eventRepo.GetUpcomingAsync(userId, days);

    public async Task<CalendarEvent> CreateAsync(CalendarEvent calendarEvent)
    {
        calendarEvent.CreatedAt = calendarEvent.UpdatedAt = DateTime.UtcNow;
        return await _eventRepo.AddAsync(calendarEvent);
    }

    public async Task UpdateAsync(CalendarEvent calendarEvent)
    {
        calendarEvent.UpdatedAt = DateTime.UtcNow;
        await _eventRepo.UpdateAsync(calendarEvent);
    }

    public Task DeleteAsync(Guid id) => _eventRepo.DeleteAsync(id);

    /// <summary>
    /// Returns all distinct event type strings the user has used, merged with defaults.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetUserEventTypesAsync(Guid userId)
    {
        var events = await _eventRepo.GetByUserAsync(userId);
        var userTypes = events.Select(e => e.EventType).Distinct();
        return CalendarEvent.DefaultTypes
            .Union(userTypes, StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();
    }

    /// <summary>
    /// Aggregates all date-bearing data for a month into unified CalendarItems.
    /// Includes user events, goal deadlines, task due dates, and check-ins.
    /// </summary>
    public async Task<IReadOnlyList<CalendarItem>> GetMonthItemsAsync(Guid userId, int year, int month)
    {
        var firstDay = new DateTime(year, month, 1);

        // Extend range to cover the full calendar grid (prev/next month overflow)
        var gridStart = firstDay.AddDays(-((int)firstDay.DayOfWeek + 6) % 7);
        var gridEnd = gridStart.AddDays(41); // 6 weeks

        var items = new List<CalendarItem>();

        // 1. User-created calendar events
        var events = await _eventRepo.GetByDateRangeAsync(userId, gridStart, gridEnd);
        foreach (var e in events)
        {
            items.Add(new CalendarItem
            {
                SourceId = e.Id,
                Title = e.Title,
                Description = e.Description,
                Source = CalendarItemSource.UserEvent,
                EventType = e.EventType,
                Date = e.Date,
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                IsAllDay = e.IsAllDay,
                IsCompleted = e.IsCompleted,
                Location = e.Location,
                Priority = e.Priority,
                PillarName = e.Pillar?.Name,
                GoalTitle = e.Goal?.Title,
            });
        }

        // 2. Goal deadlines and review dates
        var goals = await _goalRepo.GetByUserAsync(userId);
        foreach (var g in goals.Where(g => g.Status != GoalStatus.Completed && g.Status != GoalStatus.Dropped))
        {
            if (g.TargetDate.HasValue && g.TargetDate.Value.Date >= gridStart && g.TargetDate.Value.Date <= gridEnd)
            {
                items.Add(new CalendarItem
                {
                    SourceId = g.Id,
                    Title = $"Meta: {g.Title}",
                    Description = g.NextAction,
                    Source = CalendarItemSource.GoalDeadline,
                    Date = g.TargetDate.Value.Date,
                    IsAllDay = true,
                    Priority = g.Priority,
                    PillarName = g.Pillar?.Name,
                    GoalTitle = g.Title,
                });
            }
            if (g.ReviewDate.HasValue && g.ReviewDate.Value.Date >= gridStart && g.ReviewDate.Value.Date <= gridEnd)
            {
                items.Add(new CalendarItem
                {
                    SourceId = g.Id,
                    Title = $"Revisar: {g.Title}",
                    Source = CalendarItemSource.GoalReview,
                    Date = g.ReviewDate.Value.Date,
                    IsAllDay = true,
                    Priority = g.Priority,
                    PillarName = g.Pillar?.Name,
                    GoalTitle = g.Title,
                });
            }
        }

        // 3. Task due dates
        var tasks = await _taskRepo.GetByUserAsync(userId);
        foreach (var t in tasks.Where(t => t.DueDate.HasValue && t.Status != TaskItemStatus.Done && t.Status != TaskItemStatus.Cancelled))
        {
            if (t.DueDate!.Value.Date >= gridStart && t.DueDate.Value.Date <= gridEnd)
            {
                items.Add(new CalendarItem
                {
                    SourceId = t.Id,
                    Title = $"Tarefa: {t.Title}",
                    Source = CalendarItemSource.TaskDue,
                    Date = t.DueDate.Value.Date,
                    IsAllDay = true,
                    Priority = t.Priority,
                    GoalTitle = t.Goal?.Title,
                });
            }
        }

        // 4. Check-ins (for heatmap overlay)
        var checkIns = await _checkInRepo.GetRecentAsync(userId, 60);
        foreach (var ci in checkIns.Where(ci => ci.Date.Date >= gridStart && ci.Date.Date <= gridEnd))
        {
            items.Add(new CalendarItem
            {
                SourceId = ci.Id,
                Title = $"Check-in (E:{ci.Energy} M:{ci.Mood})",
                Source = CalendarItemSource.CheckIn,
                Date = ci.Date.Date,
                IsAllDay = true,
                CheckInEnergy = ci.Energy,
            });
        }

        return items.OrderBy(i => i.Date).ThenBy(i => i.StartTime).ToList();
    }
}
