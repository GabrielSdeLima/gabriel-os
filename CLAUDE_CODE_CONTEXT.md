# CLAUDE CODE CONTEXT — Gabriel OS

You are building **Gabriel OS**, a personal life operating system for Windows desktop.

## Project State Right Now

The project has been initialized with:
- Full documentation in `docs/` (product vision, domain model, core flows, MVP scope, architecture)
- Agent instructions in `AGENT.md` (read this first, always)
- Complete Clean Architecture solution that **compiles successfully** on .NET 10
- 4 projects: Domain, Application, Infrastructure, Presentation (WPF)
- All domain entities, enums, interfaces, repositories, EF Core configurations, and DbContext are implemented
- Basic ViewModels and Views exist for: Dashboard, Pillars, CheckIn, Journal
- Navigation shell with sidebar is working
- Default data seeder creates user + 7 pillars on first run
- SQLite database auto-creates at `%LOCALAPPDATA%/GabrielOS/gabrielos.db`

**The app builds but has NOT been run yet. First run may reveal runtime issues (DI wiring, XAML binding errors, navigation bugs). Expect to fix those first.**

---

## VERSION ROADMAP

### MVP-0 — Daily Operating Base (Target: 2 weeks)

**Goal:** A system Gabriel opens every morning and uses every day. Must be functional, not pretty.

#### MVP-0.1 — First Run & Smoke Test
- [ ] App launches without crashing
- [ ] Database creates and seeds automatically (1 user, 7 default pillars)
- [ ] Navigation between Dashboard, Pillars, CheckIn, Journal works
- [ ] Fix any XAML binding errors, DI resolution issues, or runtime exceptions
- [ ] Verify all 4 sidebar buttons navigate correctly

#### MVP-0.2 — Pillars Module (Fully Working)
- [ ] Pillar list shows all 7 default pillars with name, score, priority
- [ ] Selecting a pillar shows detail panel on the right
- [ ] User can edit: Vision, Current State, Score (1-10 slider), Priority
- [ ] Save button persists changes to SQLite
- [ ] Changes reflect immediately in the list after save
- [ ] User can add a new pillar (name required, rest optional)
- [ ] User can reorder pillars (drag or up/down buttons)
- [ ] User can archive/delete a pillar (with confirmation dialog)

#### MVP-0.3 — Daily Check-in (Fully Working)
- [ ] Check-in form with 4 sliders: Energy, Mood, Clarity, Tension (1-10 each)
- [ ] Real-time mode preview updates as sliders move
- [ ] Mode calculation logic (from `ModeCalculator.cs`):
  - Energy ≤ 3 OR Tension ≥ 8 → Protect
  - Clarity ≤ 4 AND Energy ≤ 5 → Simplify
  - Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 → Expand
  - Otherwise → Focus
- [ ] Text fields: Physical State, Top Concern, Top Priority, Free Text (all optional)
- [ ] Save persists to SQLite
- [ ] One check-in per day (upsert — editing today's overwrites, not duplicates)
- [ ] If today already has a check-in, form loads with existing values
- [ ] After save, shows confirmation with mode + description

#### MVP-0.4 — Journal (Fully Working)
- [ ] New entry form: Content (required), Title (optional), Entry Type dropdown
- [ ] Entry types: Reflection, Insight, Trigger, PatternNote, Learning, Idea, SomaticNote, Dream
- [ ] Entry list shows all entries reverse-chronological (newest first)
- [ ] Each entry card shows: type badge, date, title (if exists), content preview (truncated)
- [ ] Basic search: text filter across title and content
- [ ] Delete entry (with confirmation)
- [ ] Edit existing entry (click to expand/edit)
- [ ] Sensitive entry toggle (checkbox, default off)

#### MVP-0.5 — Minimal Dashboard
- [ ] Shows today's check-in mode and description (or "No check-in yet" message)
- [ ] Shows all pillars with name, score (or "—"), and trend
- [ ] Shows count of journal entries this week
- [ ] Refresh button or auto-refresh on navigation

#### MVP-0 Definition of Done
- App opens to dashboard showing pillar scores and today's state
- User can create, edit, and view pillars
- User can do daily check-in in < 3 minutes
- Check-in generates suggested mode
- User can write and browse journal entries
- All data persists in SQLite locally
- App compiles and runs on Windows without external dependencies
- No crashes on normal usage flow

---

### MVP-1 — Full Foundation (Target: 6-8 weeks after MVP-0)

**Goal:** All core modules operational, system generates real value as a life operating system.

#### MVP-1.1 — Goals Module
- [ ] Goal CRUD: create, edit, view, list
- [ ] Required fields: Title, Pillar (dropdown), Horizon (enum), Status (default: Idea), Priority (default: P3)
- [ ] Optional fields (progressive disclosure — show on expand): Description, Why It Matters, Success Criteria, Next Action, Main Risk, Start Date, Target Date, Review Date
- [ ] Horizons: Vision, Annual, Quarterly, Monthly, Sprint, Exploratory
- [ ] Statuses: Idea, Incubating, Active, Paused, Blocked, Completed, Dropped
- [ ] Priorities: P1, P2, P3, P4
- [ ] **Business rule: Max 3 P1 Active goals.** On attempting to create/promote a 4th, show warning dialog: "You already have 3 P1 active goals. Demote or pause one first."
- [ ] **Business rule: Active goals without NextAction are visually flagged** (icon or text indicator)
- [ ] **Business rule: Active goals must have ReviewDate** — warn if missing on save
- [ ] Filter/sort by: status, priority, horizon, pillar
- [ ] Goal detail view shows linked pillar, initiatives, tasks, journal entries

#### MVP-1.2 — Decisions Module
- [ ] Decision CRUD: create, edit, view, list
- [ ] Required fields: Title, Context
- [ ] Optional fields: Problem Statement, Options (structured text or JSON), Chosen Option, Rationale, Trade-offs, Risks Accepted, Review Date, Outcome Notes
- [ ] Status: Active, UnderReview, Superseded, Archived
- [ ] Link to Pillar (optional dropdown)
- [ ] Link to Goal (optional dropdown)
- [ ] **Business rule: If a decision has ReviewDate in the past, flag it on dashboard**
- [ ] Decision history/list view reverse-chronological
- [ ] Search across title and context

#### MVP-1.3 — Weekly Review Module
- [ ] Weekly review form with structured fields:
  - Wins (free text)
  - Frictions (free text)
  - Avoided Things (free text)
  - Energy Drains (free text)
  - Energy Gains (free text)
  - Main Insight (free text)
  - Next Week Focus (free text)
  - Pillar Scores snapshot (quick 1-10 per pillar, optional)
  - Notes (free text)
- [ ] One review per week (keyed by Monday of the week)
- [ ] If pillar scores are updated in review, update Pillar.Score and Pillar.LastReviewedAt
- [ ] Review history list (reverse chronological)
- [ ] View past reviews in read mode
- [ ] Completing a review should feel like < 15 minutes

#### MVP-1.4 — Cycle Focus Module
- [ ] Cycle Focus CRUD: create, edit, view
- [ ] Fields: Title, Thesis (one paragraph), Start Date, End Date, IsActive
- [ ] Link 2-3 goals as cycle priority fronts (many-to-many via CycleFocusGoal)
- [ ] **Business rule: Only one CycleFocus can be active at a time.** Activating a new one deactivates the previous.
- [ ] **Business rule: Warn if more than 3 goals are linked**
- [ ] Active cycle focus displayed prominently on Dashboard
- [ ] Linked goals get visual priority indicator in Goals list

#### MVP-1.5 — Tasks Module
- [ ] Task CRUD: create, edit, delete, list
- [ ] Fields: Title (required), Description, Status, Priority, Due Date, IsNextAction flag
- [ ] Status: Backlog, Next, Doing, Done, Cancelled
- [ ] Optional link to Goal and/or Initiative
- [ ] Simple list view with status filter
- [ ] Mark as done (quick toggle)
- [ ] Tasks linked to a goal appear in goal detail view

#### MVP-1.6 — Patterns Module
- [ ] Pattern CRUD: create, edit, view, list
- [ ] Fields: Name (required), Description, Trigger, Status
- [ ] Status: Emerging, Confirmed, Resolved
- [ ] Simple list view
- [ ] **v1 patterns are just structured notes. No confidence scoring, no evidence links, no suggested responses.**

#### MVP-1.7 — Metrics Module
- [ ] Metric recording: Name, Value, Unit, Date, Pillar link, Notes
- [ ] Record values per pillar (e.g., Weight: 82kg, Training days: 4)
- [ ] Simple list/table view per pillar
- [ ] Ability to see recent values for a metric

#### MVP-1.8 — Dashboard v2 (Full)
- [ ] Everything from MVP-0.5 plus:
- [ ] Active Cycle Focus with title, thesis, and linked goals
- [ ] Top 3 P1 active goals with status and next action
- [ ] Alert system (from `AlertRules.cs`):
  - A1: Too many P1 fronts (>3 P1 Active) → High severity
  - A2: Goal without next action (Active, NextAction empty) → Medium
  - A3: Stale goal (Active, no update in 21+ days) → Medium
  - A4: Neglected pillar (not reviewed in 30+ days) → Medium
  - A5: Low energy streak (3+ consecutive check-ins with Energy ≤ 4) → High
  - A6: Missing/overdue weekly review → Low
  - A7: Decision with ReviewDate in the past → Medium
- [ ] Alerts displayed as cards with severity color coding
- [ ] "What you're probably avoiding" section (goals/tasks stale the longest)
- [ ] "Real movement this week" section (goals updated, tasks completed, entries written)
- [ ] Last weekly review summary snippet

#### MVP-1.9 — Search & Filters
- [ ] Global search bar (accessible from any screen, keyboard shortcut Ctrl+K)
- [ ] Searches across: Goals (title, description, next action), Decisions (title, context, chosen option), Journal entries (title, content), Patterns (name, description), Weekly reviews (all text fields), Tasks (title, description)
- [ ] Results list with entity type indicator, date, and content snippet
- [ ] Click result navigates to detail view
- [ ] Filters: by entity type, by pillar, by date range, by status, by tags

#### MVP-1.10 — Export & Backup
- [ ] Manual backup: copies SQLite file to user-chosen directory with timestamp
- [ ] Last backup date tracked, warn on dashboard if > 7 days old
- [ ] Export formats:
  - JSON: full database export, all entities
  - Markdown: journal entries and decisions as .md files
  - CSV: metrics and check-in data
- [ ] Settings screen for backup directory configuration

#### MVP-1.11 — Navigation & UX Polish
- [ ] Add sidebar items for all new modules: Goals, Decisions, Reviews, Cycle Focus, Tasks, Patterns, Metrics, Search, Settings
- [ ] Active sidebar item highlighted
- [ ] Breadcrumb or back button for detail views
- [ ] Keyboard shortcuts: Ctrl+N (new entry based on current screen), Ctrl+K (search), Ctrl+D (dashboard)
- [ ] Empty states for all lists ("No goals yet. Create your first front.")
- [ ] Loading indicators where async operations happen
- [ ] Confirmation dialogs for all destructive actions (delete, archive)
- [ ] Toast notifications for saves ("Saved", "Check-in recorded", etc.)

#### MVP-1 Definition of Done
- All MVP-0 criteria still pass
- User can manage goals by horizon and priority
- System enforces max 3 P1 Active goals with warning
- User can register and review decisions
- User can complete a weekly review in < 15 minutes
- User can define a cycle focus with linked goals
- Dashboard shows alerts for stale goals, neglected pillars, overloaded P1s
- User can search across all entities
- User can export full backup
- All business rules from domain model are enforced
- No crashes on normal usage flow

---

### v2 — Intelligence & Trends (After MVP-1 is stable and used daily for 4+ weeks)

**Goal:** The system starts to see patterns and help you think, not just store.

#### v2.1 — Trend Visualization
- [ ] Pillar score history chart (line chart over weeks/months)
- [ ] Check-in trends: energy, mood, clarity, tension over time
- [ ] Goal completion rate per quarter
- [ ] Weekly review streak tracker

#### v2.2 — Monthly Review
- [ ] Monthly review as formal entity
- [ ] Evaluates: score evolution per pillar, goals completed/stale, coerência between intention and reality, patterns repeated
- [ ] Generates: thesis for next month, fronts to continue/kill, pillars to prioritize

#### v2.3 — Pattern Detection Suggestions
- [ ] Semi-automatic: system notices recurring themes in journal entries
- [ ] Suggests creating or updating patterns based on repeated keywords/themes
- [ ] "You've mentioned [topic] in 5 entries this month. Create a pattern?"

#### v2.4 — LLM-Powered Weekly Summary
- [ ] Integration with Anthropic API (Claude Sonnet)
- [ ] On weekly review, option to "Generate AI Summary"
- [ ] AI reads: week's check-ins, journal entries, goal updates, tasks completed
- [ ] Outputs: synthesis of the week, patterns noticed, suggested focus, risks
- [ ] User can accept, edit, or discard the summary
- [ ] **AI is assistive, not autonomous. User always reviews and decides.**

#### v2.5 — Computed Scoring
- [ ] Pillar scores computed from combination of:
  - Manual self-assessment (weighted highest)
  - Objective metrics (if available)
  - Goal progress in that pillar
  - Consistency of check-ins
  - Recency of review
- [ ] Coherence score: difference between declared priorities and actual energy/execution
- [ ] Stability score: based on check-in trends (energy, tension, clarity averages)
- [ ] Execution score: weekly priorities moved vs planned
- [ ] **Scores are instruments of navigation, not moral judgments**

#### v2.6 — Power User Features
- [ ] Keyboard-first navigation throughout the app
- [ ] Quick capture (global hotkey to create journal entry without opening app fully)
- [ ] Templates for journal entries (e.g., "Morning Reflection" template with prompts)
- [ ] Pin important goals/decisions to dashboard
- [ ] Archive view for completed/dropped goals and superseded decisions
- [ ] Dark/light theme toggle

---

### v3+ — Full Intelligence Layer (Future)

**Goal:** Gabriel OS becomes a genuine cognitive partner.

#### v3.1 — AI Agent Layer
- [ ] Agent roles with clear functions:
  - **Analyst:** finds patterns across journal, check-ins, goals. Detects themes, recurring blocks, energy correlations
  - **Strategist:** helps prioritize, suggests 90-day focus, proposes trade-offs, tests scenarios
  - **Operator:** breaks goals into action plans, suggests weekly structure, creates task breakdowns
  - **Auditor:** checks coherence between intentions and behavior, flags promises without execution, detects overload
  - **Mirror:** synthesizes emotional/psychological state, surfaces themes the user might not see
  - **Archivist:** connects current entries to past ones, prevents loss of context, surfaces relevant history
- [ ] Each agent has a defined interface in Application layer
- [ ] Agents operate through Application services, never touch Domain directly
- [ ] Context assembly pipeline: what data each agent receives
- [ ] User can invoke agents explicitly ("Analyze my week", "Help me prioritize")
- [ ] Agent suggestions stored as `AgentSuggestion` entities (accept/reject tracking)

#### v3.2 — Scenario Simulation
- [ ] "What if" tool: simulate choosing different paths
- [ ] Input: current state, proposed decision, constraints
- [ ] Output: projected impact on pillars, goals, trade-offs
- [ ] Stored as exploration, not prescription

#### v3.3 — Calendar Integration
- [ ] Read from Google Calendar / Outlook
- [ ] Show upcoming events alongside daily check-in
- [ ] Block time suggestions based on active goals and energy patterns
- [ ] No write access initially (read-only integration)

#### v3.4 — Mobile Companion (Read-Only)
- [ ] Simple mobile app or PWA
- [ ] View dashboard, do check-in, write quick journal entry
- [ ] No full editing capabilities — desktop remains primary
- [ ] Sync via cloud storage (encrypted)

#### v3.5 — Security & Encryption
- [ ] SQLite Cipher for full database encryption
- [ ] Per-entry encryption for sensitive journal entries
- [ ] Master password on app launch
- [ ] Encrypted cloud backup option

---

## TECHNICAL RULES (Always Follow)

1. **Domain project has ZERO external dependencies.** Pure C# POCOs.
2. **ViewModels never touch DbContext.** They call Application services.
3. **All EF Core configuration via Fluent API** in Infrastructure, never data annotations on entities.
4. **One feature at a time.** Never build multiple modules in a single pass.
5. **Project must compile after every change.** Never leave it broken.
6. **Business rules are enforced, not suggested.** Max 3 P1 goals = hard block with warning.
7. **Read relevant docs before implementing.** Always check `docs/` and `AGENT.md`.
8. **Update docs if implementation changes the model.** Docs are source of truth.
9. **Conventional commits:** `feat(domain):`, `feat(ui):`, `fix(checkin):`, `docs(model):`, etc.
10. **No scope invention.** If it's not in this roadmap, don't build it. Log it as suggestion.

## CURRENT PHASE

**MVP-0.1 — First Run & Smoke Test**

The immediate next step is: run the app (`dotnet run --project GabrielOS.Presentation`), find and fix any runtime issues, verify navigation works, verify database seeds correctly.
