namespace GabrielOS.Domain.Enums;

public enum PillarPriority
{
    Low,
    Medium,
    High,
    Critical
}

public enum Trend
{
    Unknown,
    Declining,
    Stable,
    Improving
}

public enum HorizonType
{
    Vision,
    Annual,
    Quarterly,
    Monthly,
    Sprint,
    Exploratory
}

public enum GoalStatus
{
    Idea,
    Incubating,
    Active,
    Paused,
    Blocked,
    Completed,
    Dropped
}

public enum GoalPriority
{
    P1,
    P2,
    P3,
    P4
}

public enum DecisionStatus
{
    Active,
    UnderReview,
    Superseded,
    Archived
}

public enum SuggestedMode
{
    Protect,
    Simplify,
    Focus,
    Expand
}

public enum EntryType
{
    Reflection,
    Insight,
    Trigger,
    PatternNote,
    Learning,
    Idea,
    SomaticNote,
    Dream
}

public enum PatternStatus
{
    Emerging,
    Confirmed,
    Resolved
}

public enum TaskItemStatus
{
    Backlog,
    Next,
    Doing,
    Done,
    Cancelled
}
