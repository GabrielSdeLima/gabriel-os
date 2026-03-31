# GabrielOS — User Manual

GabrielOS is your personal life operating system: a desktop app that helps you track the 7 pillars of your life, set meaningful goals, reflect weekly and monthly, and make deliberate decisions about where your energy goes.

Everything is stored locally on your machine. No cloud. No subscriptions required to use the core features.

---

## Table of Contents

1. [Philosophy](#1-philosophy)
2. [Getting Started](#2-getting-started)
3. [Navigation](#3-navigation)
4. [CORE — Daily Use](#4-core--daily-use)
   - [Dashboard](#41-dashboard)
   - [Daily Check-in](#42-daily-check-in)
   - [Journal](#43-journal)
5. [PLANNING — Direction](#5-planning--direction)
   - [Pillars](#51-pillars)
   - [Goals](#52-goals)
   - [Tasks](#53-tasks)
   - [Cycle Focus](#54-cycle-focus)
6. [REFLECTION — Sense-Making](#6-reflection--sense-making)
   - [Weekly Review](#61-weekly-review)
   - [Monthly Review](#62-monthly-review)
   - [Decisions](#63-decisions)
   - [Patterns](#64-patterns)
7. [ANALYTICS — Visibility](#7-analytics--visibility)
   - [Metrics](#71-metrics)
   - [Pillar Trends](#72-pillar-trends)
8. [SYSTEM — Configuration](#8-system--configuration)
   - [Export](#81-export)
   - [Settings](#82-settings)
9. [AI Features](#9-ai-features)
10. [Business Rules Reference](#10-business-rules-reference)
11. [Data & Privacy](#11-data--privacy)
12. [Keyboard Shortcuts](#12-keyboard-shortcuts)

---

## 1. Philosophy

GabrielOS is built on seven ideas:

- **Structure liberates.** Every input justifies its existence. No busywork.
- **Fewer fronts, more depth.** The system actively pushes back when you spread too thin (max 3 active P1 goals).
- **Clarity before volume.** A few high-quality entries beat many meaningless ones.
- **Reflection becomes direction.** Every analysis should convert to a decision, experiment, or next action.
- **Works on bad days.** Useful regardless of your current state. The check-in *adapts* to where you are.
- **Authorship, not control.** Conscious direction, not obsessive tracking.
- **Subjectivity matters.** Energy, clarity, mood, tension — these are first-class data, not noise.

The core loop is simple:

```
Check in daily → Reflect weekly → Review monthly → Adjust goals and focus
```

---

## 2. Getting Started

### First Launch

On first launch, GabrielOS creates a local database at:

```
%LOCALAPPDATA%\GabrielOS\gabrielos.db
```

It also seeds your account with:
- **User:** Gabriel (timezone: America/Sao_Paulo, phase: Rebuilding)
- **7 default pillars** representing the major domains of life (see below)

### The 7 Default Pillars

| # | Pillar | Description |
|---|--------|-------------|
| 1 | Body & Health | Physical health, training, sleep, nutrition, body-mind integration |
| 2 | Mind & Stability | Mental health, emotional regulation, therapy, meditation, inner peace |
| 3 | Career & Work | Professional direction, positioning, skills, income, leadership |
| 4 | Finances | Cash flow, savings, investments, financial independence |
| 5 | Relationships | Family, friendships, romantic life, social connections |
| 6 | Learning & Expansion | Courses, certifications, languages, intellectual growth |
| 7 | Freedom & Lifestyle | Autonomy, living environment, daily structure, quality of life |

You can rename, reorder, or update any pillar to match your actual life.

### Suggested First Steps

1. Go to **Pillars** and update the score (1–10) and trend for each pillar to reflect where you are today.
2. Go to **Cycle Focus** and define your current 30–90 day direction.
3. Go to **Goals** and add your 2–3 most important active goals (mark them P1).
4. Do your first **Daily Check-in**.
5. Come back weekly to complete a **Weekly Review**.

---

## 3. Navigation

The sidebar organizes all 16 screens into 5 groups:

| Group | Screens |
|-------|---------|
| CORE | Dashboard, Check-in, Journal |
| PLANNING | Pillars, Goals, Tasks, Cycle Focus |
| REFLECTION | Weekly Review, Monthly Review, Decisions, Patterns |
| ANALYTICS | Metrics, Pillar Trends |
| SYSTEM | Export, Settings |

Click any item in the sidebar to navigate. You can also use keyboard shortcuts — see [Section 12](#12-keyboard-shortcuts).

---

## 4. CORE — Daily Use

### 4.1 Dashboard

The Dashboard is your daily home screen. It shows the most important information at a glance.

**Left column:**
- **Suggested Mode** — Based on your most recent check-in (Protect / Simplify / Focus / Expand). See [mode logic](#mode-calculation) for details.
- **Active Cycle Focus** — Your current 30–90 day thesis and its linked goals.
- **P1 Goals** — Up to 3 of your highest-priority active goals.
- **Pillar Scores** — A quick summary of where each pillar currently stands.

**Right column (Alerts):**
- System-generated alerts that require your attention (up to 5, sorted by severity).
- Alert types include: too many P1 fronts, stale goals, missing weekly review, decisions past review date, and more.
- An empty alerts panel means the system sees no immediate issues.

### 4.2 Daily Check-in

The check-in is a 3-minute daily pulse. It captures 4 subjective signals on a 1–10 scale:

| Signal | What it measures | Low = | High = |
|--------|-----------------|-------|--------|
| **Energy** | Physical/mental vitality | Depleted | Fully charged |
| **Mood** | Emotional tone | Difficult | Positive |
| **Clarity** | Mental focus | Foggy | Sharp |
| **Tension** | Stress/pressure | Relaxed | Overloaded |

Move the sliders to reflect how you actually feel right now — not how you want to feel.

**Optional fields:**
- **Physical state** — What's happening in your body (e.g., "sore from training", "good sleep")
- **Concern** — What's on your mind or weighing on you
- **Today's priority** — The one thing that matters most today
- **Notes** — Anything else

**Mode suggestion** updates live as you move the sliders. After saving, you'll see a short description of what the suggested mode means for today.

Only one check-in per day is stored. Saving again on the same day updates the existing entry.

### Mode Calculation

| Condition | Suggested Mode | Meaning |
|-----------|---------------|---------|
| Energy ≤ 3 OR Tension ≥ 8 | **PROTECT** | Recover. Reduce load, protect capacity. |
| Clarity ≤ 4 AND Energy ≤ 5 | **SIMPLIFY** | Do less. Remove friction, lower cognitive load. |
| Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 | **EXPAND** | High capacity. Good time for new challenges. |
| Otherwise | **FOCUS** | Default operating mode. Execute on current priorities. |

### 4.3 Journal

The journal is a flexible space for anything that doesn't fit elsewhere. Each entry has an **entry type** that gives it meaning:

| Type | Use it for |
|------|-----------|
| **Reflection** | General thinking and self-observation |
| **Insight** | A realization or discovery worth keeping |
| **Trigger** | Something that provoked a strong emotional reaction |
| **Pattern Note** | A behavior or tendency you noticed repeating |
| **Learning** | Something you learned (from a book, experience, conversation) |
| **Idea** | A concept, project idea, or creative thought |
| **Somatic Note** | Body sensations, physical states, nervous system observations |
| **Dream** | A dream you want to remember or explore |

**Title** is optional but helps with search later.

**Sensitive flag** — Mark an entry as sensitive to add a confirmation gate before it's displayed. Useful for deeply personal content.

Journal entries appear in reverse chronological order. Use **Search** (Ctrl+K) to find entries by keyword.

---

## 5. PLANNING — Direction

### 5.1 Pillars

Pillars are the 7 (or more) domains that make up your life. They are the backbone of the system — everything else connects back to them.

**Fields:**

| Field | Description |
|-------|-------------|
| **Name** | The pillar's label (e.g., "Body & Health") |
| **Score** | Current state, 1–10. Update periodically. |
| **Priority** | Low / Medium / High / Critical — how much attention this pillar needs now |
| **Trend** | Unknown / Declining / Stable / Improving — direction of travel |
| **Vision** | What this pillar looks like when it's thriving |
| **Current State** | Where it actually is right now |

You can reorder pillars by using the up/down controls. The order affects how they appear throughout the app.

**Tip:** Update pillar scores after each weekly review. The Pillar Trends chart uses these historical snapshots.

### 5.2 Goals

Goals are your commitments to change something. They live within a **horizon** — a timeframe that defines their nature.

**Horizons:**

| Horizon | Timeframe | Use for |
|---------|-----------|---------|
| **Vision** | Lifelong | Big picture direction, identity-level aspirations |
| **Annual** | 1 year | What you intend to accomplish this year |
| **Quarterly** | 3 months | Focused outcomes for the current quarter |
| **Monthly** | 1 month | Specific this-month commitments |
| **Sprint** | 1–2 weeks | Short execution cycles |
| **Exploratory** | Open | Experiments, things you're trying out |

**Statuses:**

| Status | Meaning |
|--------|---------|
| Idea | Not started, just captured |
| Incubating | Thinking about it but not committed |
| Active | Currently working on it |
| Paused | Temporarily suspended |
| Blocked | Waiting on something external |
| Completed | Done |
| Dropped | Intentionally abandoned |

**Priority:**

| Priority | Meaning |
|----------|---------|
| **P1** | Highest priority — you will push almost anything aside for this |
| **P2** | Important, but yields to P1 |
| **P3** | Worth doing when P1/P2 allow |
| **P4** | Low priority / someday |

**P1 Cap rule:** You can have at most **3 active P1 goals** at any time. If you try to set a 4th, the system will block it and ask you to demote or pause one first. This is intentional — it forces real prioritization.

**Important fields:**
- **Next Action** — The concrete next physical step. Keep this current.
- **Review Date** — When you plan to reassess this goal. The alert system watches this.

### 5.3 Tasks

Tasks are concrete next actions — the smallest unit of work. They're different from Goals: a goal is an outcome you want; a task is something you do.

**Statuses:**

| Status | Meaning |
|--------|---------|
| **Backlog** | Captured but not scheduled |
| **Next** | Up next — your immediate queue |
| **Doing** | Currently in progress |
| **Done** | Completed |
| **Cancelled** | No longer relevant |

**Linking to Goals** — You can associate a task with an active goal to maintain traceability.

**Next Action flag** — Mark a task as the next action for its linked goal.

Use the **Done** toggle to quickly complete tasks without navigating to an edit form.

### 5.4 Cycle Focus

A Cycle Focus is your operating thesis for the next 30–90 days. It answers: *"Given where I am right now, what's my primary direction?"*

**Rules:**
- Only **one active cycle focus** at a time. Creating a new active cycle automatically deactivates the previous one.
- Link 2–3 goals to your cycle to make the connection between direction and execution explicit.

**Fields:**
- **Title** — Short label for this cycle
- **Thesis** — One paragraph explaining your direction and why it matters now
- **Start / End dates** — The cycle window (typically 30–90 days)
- **Linked Goals** — 2–3 active goals that this cycle serves

Your active cycle is shown on the Dashboard. If no cycle is active, that's a signal to define your current direction.

---

## 6. REFLECTION — Sense-Making

### 6.1 Weekly Review

The Weekly Review is the most important recurring practice in GabrielOS. It closes the loop on the past week and sets direction for the next.

**When:** Every week, ideally Sunday evening or Monday morning.

**The 7 review fields:**

| Field | Prompt |
|-------|--------|
| **Wins** | What went well this week? What are you proud of? |
| **Frictions** | What created resistance or slowed you down? |
| **Avoided Things** | What did you procrastinate or avoid? |
| **Energy Drains** | What depleted your energy this week? |
| **Energy Gains** | What gave you energy or felt good? |
| **Main Insight** | The single most important thing you learned or realized |
| **Next Week Focus** | Your primary intention for the coming week |

**Pillar Scores section** — Update each pillar's score (1–10) for the week. These scores are saved with the review and drive the Pillar Trends chart.

**AI Summary** — If you've configured an Anthropic API key (see [Section 9](#9-ai-features)), you can generate an AI-written summary of your week based on your check-ins, pillar scores, and active goals. Click "Generate" to produce it.

**History panel** — The right side shows your past 8 weekly reviews for context.

One review per Monday–Sunday week. Saving again overwrites the current week's entry.

### 6.2 Monthly Review

The Monthly Review zooms out to see the bigger arc. Use it at the end of each month.

**Fields:**

| Field | Prompt |
|-------|--------|
| **Highlights** | Best moments, biggest wins, what worked |
| **Lowlights** | What was hard, what failed, what fell flat |
| **Key Learnings** | The most important insights from this month |
| **Next Month Intentions** | What you intend to do differently or focus on |

**Pillar Scores** — Rate each pillar for the month.

**Month/Year picker** — Navigate to any previous month to view or edit its review.

**AI Summary** — Generate an AI analysis of the full month. Requires API key configuration.

### 6.3 Decisions

Decisions is a registry of significant choices you've made or are making. The goal is to make your decision-making process visible and reviewable.

**Fields:**

| Field | Purpose |
|-------|---------|
| **Title** | Short name for the decision |
| **Context** | Background — what situation led to this decision? |
| **Problem Statement** | What exactly are you deciding? |
| **Options** | The alternatives you considered |
| **Chosen Option** | What you decided |
| **Rationale** | Why this option over the others |
| **Trade-offs** | What you're giving up or accepting |
| **Risks Accepted** | What could go wrong that you're knowingly accepting |
| **Review Date** | When to revisit this decision — has the situation changed? |

**Status:** Active / Under Review / Superseded / Archived

**Alert A7** fires when a decision's review date has passed. This keeps decisions from becoming invisible commitments.

### 6.4 Patterns

Patterns are recurring behaviors, reactions, or tendencies you've noticed in yourself. Naming them is the first step to working with them.

**Statuses:**

| Status | Meaning |
|--------|---------|
| **Emerging** | You've noticed it a few times — watching to confirm |
| **Confirmed** | It's definitely a pattern you exhibit |
| **Resolved** | You've worked through it or it's no longer active |

**Fields:** Name, description of the pattern, trigger (what tends to set it off).

Add notes to a pattern to track observations over time.

---

## 7. ANALYTICS — Visibility

### 7.1 Metrics

Metrics are objective, quantitative data points you want to track over time. Unlike pillar scores (which are subjective), metrics are numbers.

**Examples:**
- Training days per week
- Weight (kg)
- Hours of sleep
- Books read
- Revenue (monthly)

Each metric entry has: pillar, name, value, unit, date, notes.

To track a metric over time, add a new entry each time you record it. Entries are stored separately (not overwritten), giving you a historical record.

### 7.2 Pillar Trends

The Pillar Trends screen shows a line chart of your pillar scores over time. Each pillar gets its own colored line.

**Week range selector:** 4 / 8 / 12 / 24 weeks

Data points come from the pillar scores saved in your Weekly Reviews. If you haven't been doing weekly reviews consistently, this chart will have gaps.

Use this screen to spot:
- Long-term decline in a pillar that looks fine day-to-day
- Which pillars are trending up vs. stuck
- Correlations between pillar trajectories

---

## 8. SYSTEM — Configuration

### 8.1 Export

Three export formats are available:

| Format | What it contains | File name |
|--------|-----------------|-----------|
| **JSON Snapshot** | All your data (pillars, goals, decisions, check-ins, journal, reviews, patterns, metrics, tasks) | `gabrielos_export_YYYYMMDD_HHMMSS.json` |
| **Journal Markdown** | Up to 1,000 most recent journal entries, formatted for reading | `journal_YYYYMMDD_HHMMSS.md` |
| **SQLite Backup** | A full copy of the database file | `gabrielos_backup_YYYYMMDD_HHMMSS.db` |

Click the corresponding button and choose where to save the file. All exports are one-time operations — they don't affect your live data.

**Recommended practice:** Do a SQLite backup before deleting the database or making major changes. The JSON export is good for external use (sharing, archiving, feeding to other tools).

### 8.2 Settings

The Settings screen has two sections:

**AI Configuration:**
- **API Key** — Your Anthropic API key. Stored locally in `%LOCALAPPDATA%\GabrielOS\settings.json`. Never sent anywhere except directly to Anthropic's API.
- **AI Model** — Which Claude model to use for AI summaries:

| Model | Speed | Quality | Best for |
|-------|-------|---------|---------|
| `claude-haiku-4-5` | Fastest | Good | Daily use, quick summaries |
| `claude-sonnet-4-6` | Balanced | Excellent | Default recommendation |
| `claude-opus-4-6` | Slower | Best | Deep analysis, complex reviews |

Click **Save** to apply. Click **Clear** to remove the API key and disable AI features.

---

## 9. AI Features

GabrielOS can connect to Anthropic's Claude API to generate written summaries of your weekly and monthly reviews. This is **completely optional** — the app works fully without it.

### Getting an API Key

1. Go to [console.anthropic.com](https://console.anthropic.com)
2. Create an account or sign in
3. Navigate to **API Keys** and create a new key
4. Copy the key (it starts with `sk-ant-...`)

### Configuring the Key

1. Open **Settings** (sidebar or Ctrl+,)
2. Paste your API key in the API Key field
3. Choose your preferred model (Sonnet is recommended)
4. Click **Save**

The AI features will now be available on the Weekly Review and Monthly Review screens.

### What the AI Does

**Weekly Review AI Summary:**
The AI receives your week's check-ins (energy, mood, clarity, tension values), your pillar scores, your active goals, and the text you wrote in the review fields. It generates a 2–4 paragraph prose summary identifying patterns in your week, what's working, and areas to watch.

**Monthly Review AI Summary:**
The AI receives all weekly reviews from the selected month, your monthly review fields, and your active goals. It generates a 3–5 paragraph analysis covering the month's arc, patterns, what worked, what needs attention, and actionable suggestions for next month.

### Cost

API calls use Anthropic's pay-per-use pricing. Haiku is the cheapest model and is well-suited for these summaries. A typical weekly review generation costs a fraction of a cent. If you have the Max subscription on claude.ai, note that this does **not** cover API usage — the API is billed separately through console.anthropic.com.

---

## 10. Business Rules Reference

### P1 Goal Cap

- Maximum **3 active P1 goals** at any time.
- The system blocks creation or update of a 4th P1 active goal.
- To add a new P1: pause, complete, drop, or demote an existing one.

### Mode Calculation

Calculated from your most recent check-in values:

| Condition | Mode |
|-----------|------|
| Energy ≤ 3 OR Tension ≥ 8 | **PROTECT** — Recover. Protect capacity. Reduce load. |
| Clarity ≤ 4 AND Energy ≤ 5 | **SIMPLIFY** — Do less. Lower cognitive load. |
| Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 | **EXPAND** — High capacity. Push forward on big things. |
| Everything else | **FOCUS** — Default mode. Execute on current priorities. |

### Alert System

Alerts appear on the Dashboard (up to 5, highest severity first):

| Alert | Trigger | Severity |
|-------|---------|----------|
| **A1 — Too many P1 fronts** | More than 3 active P1 goals | High |
| **A2 — Goal without next action** | Active goal with no Next Action defined | Medium |
| **A3 — Stale goal** | Active goal with no update in 21+ days | Medium |
| **A4 — Neglected pillar** | Pillar not reviewed in 30+ days | Medium |
| **A5 — Low energy streak** | 3+ consecutive check-ins with Energy ≤ 4 | High |
| **A6 — Missing weekly review** | No review yet, or 9+ days since last | Low |
| **A7 — Decision needs review** | Decision with a review date in the past | Medium |

Resolving an alert means fixing the underlying condition (e.g., adding a next action to a stale goal), not dismissing the alert.

### Other Rules

- **One active Cycle Focus at a time.** Creating a new active cycle deactivates the previous one automatically.
- **One check-in per day.** Saving again on the same day updates the existing entry.
- **One weekly review per week.** Saving again during the same Monday–Sunday week overwrites the entry.

---

## 11. Data & Privacy

- All data is stored **locally** on your machine at `%LOCALAPPDATA%\GabrielOS\`
- The database is a standard SQLite file: `gabrielos.db`
- Settings (including your API key) are stored in `settings.json` in the same folder
- Nothing is sent to external servers unless you've configured an API key and trigger an AI generation
- When AI features are used, the context sent to Anthropic includes your check-in numbers, pillar scores, goal titles, and review text — no account information or identifying data beyond what you've written

**To back up your data:** Use the SQLite Backup option in the Export screen.

**To move to a new machine:** Copy the `%LOCALAPPDATA%\GabrielOS\` folder to the same path on the new machine, then install GabrielOS.

**To start fresh:** Delete `gabrielos.db`. The app will re-create it with default data on next launch. Your settings (API key, model) will be preserved unless you also delete `settings.json`.

---

## 12. Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `Ctrl+D` | Go to Dashboard |
| `Ctrl+J` | Go to Journal |
| `Ctrl+G` | Go to Goals |
| `Ctrl+K` | Open Search |
| `Ctrl+,` | Go to Settings |

---

*GabrielOS — built for clarity, not volume.*
