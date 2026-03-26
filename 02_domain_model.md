# Gabriel OS — Domain Model

## 1. Core Concepts (Internal Naming)

| Concept | Internal Name | Description |
|---------|---------------|-------------|
| Pillar | `Pillar` | A core life domain (Body, Career, Mind, etc.) |
| Front | `Goal` | An active objective with horizon and priority |
| Initiative | `Initiative` | A sub-project under a Goal |
| Decision | `Decision` | A structured strategic choice |
| Check-in | `CheckIn` | Daily state snapshot |
| Entry | `JournalEntry` | A reflection, insight, pattern note, or learning |
| Review | `WeeklyReview` | Structured weekly retrospective |
| Cycle Focus | `CycleFocus` | The 2-3 priority fronts for the current cycle (30-90 days) |
| Pattern | `Pattern` | A recurring behavior, trigger, or tendency |
| Metric | `Metric` | An objective data point tied to a pillar |
| Task | `Task` | A concrete next action |

## 2. Entity Definitions

### 2.1 User

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| Name | string | Yes | |
| Timezone | string | Yes | IANA format |
| CurrentPhase | string | No | Free text: "rebuilding", "expanding", etc. |
| PreferencesJson | string | No | Serialized settings |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

### 2.2 Pillar

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| Name | string | Yes | e.g. "Body & Health" |
| Description | string | No | |
| Vision | string | No | Desired state |
| CurrentState | string | No | Free text assessment |
| Score | int | No | 1-10, manual self-assessment |
| Priority | enum | Yes | Low, Medium, High, Critical |
| Trend | enum | No | Improving, Stable, Declining, Unknown |
| SortOrder | int | Yes | Display order |
| LastReviewedAt | DateTime | No | |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

**Default Pillars (seeded on first run):**
1. Body & Health
2. Mind & Stability
3. Career & Work
4. Finances
5. Relationships
6. Learning & Expansion
7. Freedom & Lifestyle

User can add, rename, reorder, or archive pillars.

### 2.3 Goal

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| PillarId | GUID | Yes | FK → Pillar |
| Title | string | Yes | |
| Description | string | No | |
| WhyItMatters | string | No | |
| HorizonType | enum | Yes | Vision, Annual, Quarterly, Monthly, Sprint, Exploratory |
| Status | enum | Yes | Idea, Incubating, Active, Paused, Blocked, Completed, Dropped |
| Priority | enum | Yes | P1, P2, P3, P4 |
| NextAction | string | No | The single next physical action |
| SuccessCriteria | string | No | |
| MainRisk | string | No | |
| StartDate | Date | No | |
| TargetDate | Date | No | |
| ReviewDate | Date | No | |
| CompletedAt | DateTime | No | |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

**Business Rules:**
- Maximum 3 goals with Priority = P1 and Status = Active at any time.
- Active goals without NextAction are flagged as "inoperative".
- Active goals must have a ReviewDate.

### 2.4 Initiative

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| GoalId | GUID | Yes | FK → Goal |
| Title | string | Yes | |
| Description | string | No | |
| Status | enum | Yes | Same as GoalStatus |
| NextAction | string | No | |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

### 2.5 Decision

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| PillarId | GUID | No | FK → Pillar |
| GoalId | GUID | No | FK → Goal |
| Title | string | Yes | |
| Context | string | Yes | What situation prompted this |
| ProblemStatement | string | No | |
| OptionsJson | string | No | Serialized list of options considered |
| ChosenOption | string | No | |
| Rationale | string | No | Why this choice |
| TradeOffs | string | No | |
| RisksAccepted | string | No | |
| Status | enum | Yes | Active, UnderReview, Superseded, Archived |
| ReviewDate | Date | No | |
| OutcomeNotes | string | No | Filled later |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

**Business Rules:**
- If a journal entry or check-in references a topic where an active Decision already exists, the system should surface that decision.

### 2.6 CheckIn

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| Date | Date | Yes | One per day |
| Energy | int | Yes | 1-10 |
| Mood | int | Yes | 1-10 |
| Clarity | int | Yes | 1-10 |
| Tension | int | Yes | 1-10 |
| PhysicalState | string | No | Free text |
| TopConcern | string | No | |
| TopPriority | string | No | What I will focus on |
| FreeText | string | No | Anything else |
| SuggestedMode | enum | No | Protect, Simplify, Focus, Expand |
| CreatedAt | DateTime | Yes | |

**Mode Suggestion Rules (v1 — deterministic):**

| Condition | Suggested Mode |
|-----------|----------------|
| Energy ≤ 3 OR Tension ≥ 8 | Protect |
| Clarity ≤ 4 AND Energy ≤ 5 | Simplify |
| Energy ≥ 6 AND Clarity ≥ 6 | Focus |
| Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 | Expand |
| Otherwise | Focus (default) |

### 2.7 JournalEntry

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| PillarId | GUID | No | FK → Pillar |
| GoalId | GUID | No | FK → Goal |
| DecisionId | GUID | No | FK → Decision |
| EntryType | enum | Yes | Reflection, Insight, Trigger, PatternNote, Learning, Idea, SomaticNote, Dream |
| Title | string | No | |
| Content | string | Yes | |
| Mood | int | No | 1-10 |
| Energy | int | No | 1-10 |
| Intensity | int | No | 1-10 |
| TagsJson | string | No | Serialized list of tags |
| IsSensitive | bool | Yes | Default false |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

### 2.8 WeeklyReview

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| WeekStart | Date | Yes | Monday of the week |
| Wins | string | No | What went well |
| Frictions | string | No | What was hard |
| AvoidedThings | string | No | What was dodged |
| EnergyDrains | string | No | |
| EnergyGains | string | No | |
| MainInsight | string | No | |
| NextWeekFocus | string | No | |
| PillarScoresJson | string | No | Snapshot of all pillar scores |
| Notes | string | No | |
| CreatedAt | DateTime | Yes | |

### 2.9 CycleFocus

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| Title | string | Yes | e.g. "Q2 2026: Base Rebuild" |
| Thesis | string | No | One-paragraph direction statement |
| StartDate | Date | Yes | |
| EndDate | Date | Yes | |
| IsActive | bool | Yes | Only one active at a time |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

**Relationships:** CycleFocus links to Goals via a join table `CycleFocusGoal(CycleFocusId, GoalId)`.

### 2.10 Pattern

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| Name | string | Yes | e.g. "Avoids uncomfortable study" |
| Description | string | No | |
| Trigger | string | No | What activates it |
| Status | enum | Yes | Emerging, Confirmed, Resolved |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

**v1 scope:** Patterns are simple tagged notes. No confidence scoring, no evidence links, no suggested responses. Those come in v2.

### 2.11 Metric

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| PillarId | GUID | Yes | FK → Pillar |
| Name | string | Yes | e.g. "Weight", "Training days/week" |
| Value | decimal | Yes | |
| Unit | string | No | |
| Date | Date | Yes | |
| Notes | string | No | |
| CreatedAt | DateTime | Yes | |

### 2.12 Task

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| Id | GUID | Yes | PK |
| UserId | GUID | Yes | FK → User |
| GoalId | GUID | No | FK → Goal |
| InitiativeId | GUID | No | FK → Initiative |
| Title | string | Yes | |
| Description | string | No | |
| Status | enum | Yes | Backlog, Next, Doing, Done, Cancelled |
| Priority | enum | No | P1, P2, P3, P4 |
| DueDate | Date | No | |
| IsNextAction | bool | Yes | Default false |
| CreatedAt | DateTime | Yes | |
| UpdatedAt | DateTime | Yes | |

## 3. Enums Summary

```
PillarPriority: Low, Medium, High, Critical
Trend: Improving, Stable, Declining, Unknown
HorizonType: Vision, Annual, Quarterly, Monthly, Sprint, Exploratory
GoalStatus: Idea, Incubating, Active, Paused, Blocked, Completed, Dropped
GoalPriority: P1, P2, P3, P4
DecisionStatus: Active, UnderReview, Superseded, Archived
SuggestedMode: Protect, Simplify, Focus, Expand
EntryType: Reflection, Insight, Trigger, PatternNote, Learning, Idea, SomaticNote, Dream
PatternStatus: Emerging, Confirmed, Resolved
TaskStatus: Backlog, Next, Doing, Done, Cancelled
```

## 4. Entity Relationships

```
User 1──* Pillar
User 1──* Goal
User 1──* Decision
User 1──* CheckIn
User 1──* JournalEntry
User 1──* WeeklyReview
User 1──* CycleFocus
User 1──* Pattern
User 1──* Metric
User 1──* Task

Pillar 1──* Goal
Pillar 1──* Metric
Pillar 1──* JournalEntry (optional)

Goal 1──* Initiative
Goal 1──* Task
Goal *──* CycleFocus (via CycleFocusGoal)

Goal 0..1──* JournalEntry (optional)
Goal 0..1──* Decision (optional)

Decision 0..1── Pillar (optional)
Decision 0..1── Goal (optional)

Task 0..1── Goal (optional)
Task 0..1── Initiative (optional)
```

## 5. Business Rules

| # | Rule | Enforcement |
|---|------|-------------|
| BR-01 | Max 3 P1 Active goals at any time | Warn on creation, block on 4th |
| BR-02 | Active goals without NextAction are flagged | Visual indicator |
| BR-03 | Active goals must have ReviewDate | Warn if missing |
| BR-04 | One CheckIn per day | Upsert on same date |
| BR-05 | One CycleFocus active at a time | Deactivate previous on activation |
| BR-06 | Decisions on reviewed topics resurface | Query match on save |
| BR-07 | Stale goals (Active, no update in 21 days) get flagged | Dashboard alert |
| BR-08 | Sensitive journal entries require confirmation to view | UI gate |
| BR-09 | Weekly review should result in at least one focus/adjustment | Prompt, not enforce |
| BR-10 | Pillar without review in 30+ days gets elevated visibility | Dashboard alert |
