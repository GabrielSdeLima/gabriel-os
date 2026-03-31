# GabrielOS — User Manual

## Table of Contents

1. [Overview & Philosophy](#overview--philosophy)
2. [Getting Started](#getting-started)
3. [Navigation](#navigation)
4. [CORE Screens](#core-screens)
   - [Dashboard](#dashboard)
   - [Daily Check-in](#daily-check-in)
   - [Journal](#journal)
5. [PLANNING Screens](#planning-screens)
   - [Pillars](#pillars)
   - [Goals](#goals)
   - [Tasks](#tasks)
   - [Cycle Focus](#cycle-focus)
6. [REFLECTION Screens](#reflection-screens)
   - [Weekly Review](#weekly-review)
   - [Monthly Review](#monthly-review)
   - [Decisions](#decisions)
   - [Patterns](#patterns)
7. [ANALYTICS Screens](#analytics-screens)
   - [Metrics](#metrics)
   - [Pillar Trends](#pillar-trends)
8. [SYSTEM Screens](#system-screens)
   - [Export](#export)
   - [Settings](#settings)
9. [AI Features](#ai-features)
10. [Business Rules Reference](#business-rules-reference)
11. [Data & Privacy](#data--privacy)
12. [Keyboard Shortcuts](#keyboard-shortcuts)
13. [Default Pillars](#default-pillars)

---

## Overview & Philosophy

GabrielOS is a personal operating system for intentional living. It is built around **7 pillars of life** — the core areas that together define a well-rounded, purposeful existence. The app gives you a structured way to check in with yourself daily, plan with clarity, reflect honestly, and spot patterns over time.

The daily loop is simple:

1. **Check in** — Score your energy, mood, clarity, and tension. The system suggests an operating mode for the day.
2. **Act** — Work on your goals, track tasks, journal your thoughts.
3. **Reflect** — Weekly and monthly reviews help you see what's working and what needs attention.
4. **Adjust** — Update pillar scores, shift priorities, start a new cycle focus.

Everything lives locally on your machine. There is no cloud, no account, no external dependency beyond an optional AI integration for summaries.

---

## Getting Started

On first launch, GabrielOS creates a default user profile and seeds **7 foundation pillars** (Body & Health, Mind & Stability, Career & Work, Finances, Relationships, Learning & Expansion, Freedom & Lifestyle). These pillars are yours to customize — rename them, update their descriptions, or adjust their priorities as you see fit.

**Suggested first steps:**

1. Open **Check-in** and do your first daily check-in. Move the 4 sliders to match how you feel right now.
2. Go to **Pillars** and review the 7 defaults. Edit descriptions and visions to match your life.
3. Create your first **Goal** — pick a pillar, set a horizon (Quarterly or Monthly works well to start), and write a clear next action.
4. Start a **Cycle Focus** — pick a 30-day theme and link up to 3 goals to it.
5. At the end of the week, do your first **Weekly Review**.

---

## Navigation

The sidebar is organized into 5 groups:

| Group | Screens | Purpose |
|-------|---------|---------|
| **CORE** | Dashboard, Check-in, Journal | Quick daily access |
| **PLANNING** | Pillars, Goals, Tasks, Cycle Focus | Goal-oriented work |
| **REFLECTION** | Weekly Review, Monthly Review, Decisions, Patterns | Intentional retrospective |
| **ANALYTICS** | Metrics, Pillar Trends | Insights and trends |
| **SYSTEM** | Export, Settings | Configuration and data |

A **search bar** sits at the top of the sidebar (Ctrl+K). The **back button** in the header lets you return to previously visited screens (up to 8 steps of history).

If you have unsaved changes on the current screen, GabrielOS will warn you before navigating away.

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| Ctrl+D | Go to Dashboard |
| Ctrl+J | Go to Journal |
| Ctrl+G | Go to Goals |
| Ctrl+K | Open Search |
| Ctrl+, | Open Settings |
| Alt+Left | Go Back |
| Ctrl+S | Save (on any screen with a save action) |

---

## CORE Screens

### Dashboard

The Dashboard is your command center. It shows a unified snapshot of your current state across several sections:

- **Pillar Scores & Trends** — Each of your pillars with its current score and trend arrow (improving, stable, declining, or unknown).
- **P1 Active Goals** — Your top-priority goals that are currently active (up to 3 shown).
- **Today's Check-in** — Your most recent check-in metrics and the suggested operating mode.
- **Active Cycle Focus** — The title and date range of your current cycle.
- **Weekly Journal Count** — How many journal entries you've written this week.
- **Alerts** — The top 5 automated alerts that need your attention (see [Business Rules](#business-rules-reference) for all alert types).

The Dashboard is read-only — it pulls data from all other screens. To change anything, navigate to the relevant screen.

### Daily Check-in

The Check-in screen captures how you're doing right now. It has **4 slider metrics**, each on a 1–10 scale:

| Metric | What it measures |
|--------|-----------------|
| **Energy** | Physical and mental fuel — how much capacity you have today |
| **Mood** | Emotional state — how positive or negative you feel |
| **Clarity** | Mental sharpness — how well you can think and decide |
| **Tension** | Stress/pressure — how tightly wound you feel (higher = worse) |

As you move the sliders, the screen shows a **Suggested Mode** in real time:

| Mode | When it triggers | What it means |
|------|-----------------|---------------|
| **PROTECT** | Energy ≤ 3 OR Tension ≥ 8 | You're running low or overwhelmed. Minimize demands, do only what's essential. |
| **SIMPLIFY** | Clarity ≤ 4 AND Energy ≤ 5 | Your head is foggy. Pick one thing and focus on it. Cut everything else. |
| **EXPAND** | Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 | You're firing on all cylinders. Plan, explore, or tackle something ambitious. |
| **FOCUS** | Default (none of the above) | Stable state. Execute on your priorities, close open loops. |

Below the sliders, you can add optional text:
- **Physical State** — How your body feels (sore, rested, sick, etc.)
- **Top Concern** — What's weighing on your mind
- **Top Priority** — The one thing you want to accomplish today
- **Free Text** — Anything else worth noting

The **History** sidebar on the right shows your last 90 days of check-ins. Click any past check-in to view its details. Click "Back to Today" to return to the current day's form.

You can do one check-in per day. If you've already saved one today, the form loads your existing data so you can update it.

### Journal

The Journal is your free-form space for capturing thoughts, insights, and reflections. Each entry has:

- **Entry Type** — Choose from 8 types:
  - **Reflection** — General thinking or processing
  - **Insight** — An "aha" moment or realization
  - **Trigger** — Something that caused a strong reaction
  - **Pattern Note** — An observation about a recurring behavior
  - **Learning** — Something new you learned
  - **Idea** — A creative thought or possibility
  - **Somatic Note** — A body-based observation (physical sensation, tension, etc.)
  - **Dream** — A dream worth recording
- **Title** and **Content** — What happened, what you noticed, what you think about it.
- **Mood / Energy / Intensity** — Optional numeric scores to tag the entry.
- **Tags** — Free-form tags for categorization.
- **Sensitive** — Flag an entry as sensitive (for your own awareness).
- **Links** — Optionally connect the entry to a Pillar, Goal, or Decision.

The sidebar shows your recent entries. When no entries exist, an empty state message guides you to create your first one.

---

## PLANNING Screens

### Pillars

Pillars represent the foundational areas of your life. GabrielOS comes with 7 defaults (see [Default Pillars](#default-pillars)), but you can customize them fully.

Each pillar has:

- **Name** — A short label (e.g., "Body & Health")
- **Description** — What this area means to you
- **Vision** — Where you want this area to be in the future
- **Current State** — An honest assessment of where things stand now
- **Score** — A 1–10 rating you assign (updated during reviews)
- **Priority** — Low, Medium, High, or Critical
- **Trend** — Improving, Stable, Declining, or Unknown (shown as arrows on the Dashboard)
- **Last Reviewed** — When you last updated this pillar's assessment

The screen opens with the first pillar pre-selected. Click any pillar in the list to view and edit its details.

### Goals

Goals are the concrete outcomes you're working toward. Each goal is linked to a pillar and has several key fields:

- **Title** and **Description** — What you're trying to achieve
- **Why It Matters** — Your motivation (helpful during reviews)
- **Pillar** — Which life area this goal belongs to
- **Horizon** — The time scale:
  - **Vision** — Long-term (years)
  - **Annual** — This year
  - **Quarterly** — This quarter
  - **Monthly** — This month
  - **Sprint** — Short burst (1–2 weeks)
  - **Exploratory** — Open-ended investigation
- **Priority** — P1 (highest) through P4 (lowest). **Important: You can have at most 3 active P1 goals.** The system alerts you if you exceed this.
- **Status** — Idea, Incubating, Active, Paused, Blocked, Completed, Dropped
- **Next Action** — The very next concrete step. Goals without a next action trigger an alert.
- **Success Criteria** — How you'll know it's done
- **Main Risk** — What could go wrong
- **Dates** — Start, target, review, and completion dates

The sidebar shows your goals list, filtered by the selected pillar.

### Tasks

Tasks are the actionable work items that move your goals forward. Each task has:

- **Title** and **Description**
- **Status** — Backlog, Next, Doing, Done, Cancelled
- **Priority** — P1 through P4
- **Due Date** — Optional deadline
- **Is Next Action** — Flag this task as the immediate next step
- **Links** — Connect to a Goal or Initiative

The "Goal" dropdown shows your existing goals. If it appears empty, you need to create goals first.

### Cycle Focus

A Cycle Focus is a time-boxed period (typically 30 days) where you commit to a strategic theme. It answers the question: "What's the one thing I'm going to pour my energy into this month?"

Each cycle has:

- **Title** — A clear name for the focus period (required)
- **Thesis** — A short statement of what you're betting on or testing (optional but recommended)
- **Start / End Dates** — The time window for this cycle
- **Linked Goals** — Up to 3 goals that serve this cycle's theme

**Rules:**
- Only **one cycle can be active** at a time. Starting a new cycle automatically deactivates the previous one.
- Maximum **3 goals per cycle** — this forces prioritization.
- Only goals with **Active** or **Incubating** status can be linked.

Past cycles are visible below the active cycle for reference.

---

## REFLECTION Screens

### Weekly Review

The Weekly Review is your end-of-week checkpoint. It has 7 structured fields plus pillar scoring:

| Field | Purpose |
|-------|---------|
| **Wins this week** | What went well — celebrate progress |
| **Frictions / what didn't work** | What got in the way |
| **What I avoided (and shouldn't have)** | Honest self-assessment of procrastination or avoidance |
| **Energy drains** | Activities or people that drained your energy |
| **Energy gains** | Activities or people that energized you |
| **Main insight** | The single most important takeaway from this week |
| **Next week focus** | What you want to prioritize next week |
| **Notes** | Anything else worth noting |

**Pillar Scores** — Rate each pillar from 1–10 for this week using sliders.

**AI Summary** — If you've configured an API key, click "Generate" to get an AI-powered analysis of your week (see [AI Features](#ai-features)).

The **Past Reviews** sidebar on the right shows your recent weekly reviews. Click any past review to load it and see what you wrote. The current week is shown by default.

### Monthly Review

The Monthly Review takes a wider view. It has 4 structured fields:

| Field | Purpose |
|-------|---------|
| **Highlights** | The best moments and achievements of the month |
| **Lowlights** | What was difficult or disappointing |
| **Key Learnings** | What you learned about yourself, your work, or your life |
| **Next Month Intentions** | What you want to carry forward or change |

**Pillar Scores** — Rate each pillar for the month.

Use the **month/year selectors** at the top to navigate between months. The **Past Reviews** sidebar shows your last 12 monthly reviews — click any to load it.

**AI Summary** — Available if you've configured an API key. Analyzes your monthly data and generates a personalized insight summary.

### Decisions

The Decisions screen helps you document important choices so you can learn from them later. Each decision has:

- **Title** — A short name for the decision
- **Context / Problem** — What situation led to this decision
- **Options** — The alternatives you considered
- **Chosen Option** — What you decided
- **Rationale** — Why you chose this option
- **Trade-offs** — What you're giving up
- **Risks Accepted** — Known risks you're taking on
- **Status** — Active, Under Review, Superseded, or Archived
- **Review Date** — When to revisit this decision (triggers an alert when overdue)
- **Outcome Notes** — What actually happened (filled in later)
- **Links** — Optionally connect to a Pillar or Goal

### Patterns

The Patterns screen tracks recurring behaviors you notice in yourself. Each pattern has:

- **Name** — A clear label (e.g., "Overcommitting on Mondays")
- **Description** — What the pattern looks like in practice
- **Trigger** — What sets it off
- **Status** — Three stages:
  - **Emerging** — You've noticed it but aren't sure yet
  - **Confirmed** — You've seen it enough times to know it's real
  - **Resolved** — You've addressed it or it's no longer active

---

## ANALYTICS Screens

### Metrics

The Metrics screen lets you track quantitative data points for each pillar. Each metric entry has:

- **Pillar** — Which life area this metric belongs to
- **Name** — What you're measuring (e.g., "Hours of sleep", "Books read")
- **Value** — The number
- **Unit** — The unit of measurement (e.g., "hours", "kg", "count")
- **Date** — When the measurement was taken
- **Notes** — Additional context

Use metrics to track objective data that supplements the subjective pillar scores from your reviews.

### Pillar Trends

The Pillar Trends screen shows a visual chart of your pillar scores over time using line graphs. You can select different time ranges to view:

- **4 weeks** — Short-term trends
- **8 weeks** — Medium-term view
- **12 weeks** — Quarterly perspective
- **24 weeks** — Half-year view

This helps you see which pillars are improving, which are stable, and which need attention.

---

## SYSTEM Screens

### Export

The Export screen offers 3 ways to back up your data:

| Format | Description | Filename |
|--------|-------------|----------|
| **Database Backup** | A full copy of the SQLite database file (.db). This is the most complete backup — it includes everything. | `gabrielos_backup_YYYYMMDD_HHmm.db` |
| **JSON Export** | A structured JSON file with all your pillars, goals, decisions, check-ins, journal entries, reviews, patterns, metrics, and tasks. Good for portability or analysis. | `gabrielos_export_YYYYMMDD_HHmmss.json` |
| **Journal Markdown** | A Markdown-formatted export of up to 1,000 recent journal entries. Good for reading or archiving your reflections. | `journal_YYYYMMDD_HHmmss.md` |

Export files are saved to a location you choose via a file dialog.

### Settings

The Settings screen has two options:

1. **API Key** — Enter your Anthropic API key to enable AI features. The key is stored locally. You can clear it at any time.
2. **AI Model** — Choose which Claude model to use for AI summaries:
   - **claude-haiku-4-5-20251001** — Fastest and cheapest. Good for quick summaries.
   - **claude-sonnet-4-6** — Balanced speed and quality. Recommended for most users.
   - **claude-opus-4-6** — Most capable. Best for deep, nuanced analysis.

---

## AI Features

GabrielOS has optional AI integration powered by Anthropic's Claude. AI features are **off by default** — you need to configure an API key to enable them.

### How to get an API key

1. Go to [console.anthropic.com](https://console.anthropic.com) and create an account.
2. Navigate to API Keys and create a new key.
3. Copy the key and paste it into GabrielOS Settings.

### Where AI appears

AI summaries are available on two screens:

- **Weekly Review** — Click "Generate" to get a 2–4 paragraph analysis of your week. The AI looks at your check-in data, pillar scores, journal entries, and review fields to identify patterns, highlight what's working, and suggest one concrete action.
- **Monthly Review** — Click "Generate AI Summary" for a 3–5 paragraph monthly analysis. The AI considers your monthly data and offers insights about trends, progress, and areas that need attention.

If no API key is configured, the Generate button is disabled and a message guides you to Settings.

### Which model to choose

- **Haiku** — Use this if you want fast, inexpensive summaries and don't need deep analysis.
- **Sonnet** — The sweet spot. Good quality at reasonable cost. Start here.
- **Opus** — Use this for the most thoughtful, detailed analysis. Costs more per request.

The AI writes in flowing prose (no bullet points) and speaks to you directly and personally.

---

## Business Rules Reference

### P1 Goal Cap

You can have at most **3 active P1 goals** at any time. If you exceed this, the system generates a high-priority alert (A1). This rule exists to prevent overcommitment — if everything is P1, nothing is.

### Mode Calculation

Your suggested daily mode is calculated from your check-in metrics:

| Condition | Suggested Mode |
|-----------|---------------|
| Energy ≤ 3 OR Tension ≥ 8 | **PROTECT** |
| Clarity ≤ 4 AND Energy ≤ 5 | **SIMPLIFY** |
| Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 | **EXPAND** |
| None of the above | **FOCUS** |

Conditions are evaluated in order — the first match wins.

### Alert Types

| Code | Alert | Condition | Severity |
|------|-------|-----------|----------|
| A1 | Too many P1 goals | More than 3 active P1 goals | High |
| A2 | Goal without next action | An active goal has no next action defined | Medium |
| A3 | Stale goal | An active goal hasn't been updated in 21+ days | Medium |
| A4 | Neglected pillar | A pillar hasn't been reviewed in 30+ days | Medium |
| A5 | Low energy streak | 3+ consecutive check-ins with energy ≤ 4 | High |
| A6 | Missing weekly review | No weekly review, or last one was 9+ days ago | Low |

### Cycle Focus Rules

- One active cycle at a time. Activating a new one deactivates the previous.
- Maximum 3 goals linked per cycle.
- Only Active or Incubating goals can be linked.

### Upsert Behavior

Check-ins, weekly reviews, and monthly reviews use "upsert" logic — if one already exists for the same date/week/month, saving updates the existing record rather than creating a duplicate.

---

## Data & Privacy

**All your data stays on your machine.** GabrielOS uses a local SQLite database — there is no server, no sync, no telemetry.

The database file is stored at:
```
%LOCALAPPDATA%\GabrielOS\gabrielos.db
```

The only external network call GabrielOS makes is to the Anthropic API when you explicitly click "Generate" on a review screen — and only if you've configured an API key. The data sent is limited to the context needed for that specific summary (your check-ins, pillar scores, and review fields for the relevant time period).

### Backup Strategy

Use the **Export** screen regularly to create backups:

- **Database backup** for full restoration capability
- **JSON export** for portable, readable data
- **Journal Markdown** for an archive of your reflections

Store backups in a safe location (cloud storage, external drive, etc.).

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl+D** | Dashboard |
| **Ctrl+J** | Journal |
| **Ctrl+G** | Goals |
| **Ctrl+K** | Search |
| **Ctrl+,** | Settings |
| **Alt+Left** | Go Back |
| **Ctrl+S** | Save (available on all screens with a save action) |

---

## Default Pillars

GabrielOS ships with 7 foundation pillars, each set to Medium priority:

| # | Pillar | Description |
|---|--------|-------------|
| 1 | **Body & Health** | Physical health, training, sleep, nutrition, body-mind integration |
| 2 | **Mind & Stability** | Mental health, emotional regulation, therapy, meditation, inner peace |
| 3 | **Career & Work** | Professional direction, positioning, skills, income, leadership |
| 4 | **Finances** | Cash flow, savings, investments, financial independence |
| 5 | **Relationships** | Family, friendships, romantic life, social connections |
| 6 | **Learning & Expansion** | Courses, certifications, languages, intellectual growth |
| 7 | **Freedom & Lifestyle** | Autonomy, living environment, daily structure, quality of life |

These pillars are fully customizable. Rename them, rewrite their descriptions, change priorities, or add your own vision and current state assessments to make them yours.
