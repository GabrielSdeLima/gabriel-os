# Gabriel OS — Core Flows

## 1. Daily Check-in Flow

**When:** Morning, first interaction of the day.
**Duration target:** < 3 minutes.

### Input

User fills:
1. Energy (1-10 slider)
2. Mood (1-10 slider)
3. Clarity (1-10 slider)
4. Tension (1-10 slider)
5. Physical state (optional free text)
6. Top concern (optional free text)
7. Top priority for today (optional free text)

### Processing

System determines `SuggestedMode`:

```
IF Energy ≤ 3 OR Tension ≥ 8 → Protect
ELSE IF Clarity ≤ 4 AND Energy ≤ 5 → Simplify
ELSE IF Energy ≥ 7 AND Clarity ≥ 7 AND Tension ≤ 4 → Expand
ELSE → Focus
```

### Output

Display:
- Suggested mode with one-line explanation
- Weekly focus reminder (from CycleFocus or manual)
- Top priority of the day
- One alert if applicable (stale goal, neglected pillar, overloaded P1s)

### Example

```
Mode: Simplify — low clarity today, pick one thing and protect your energy.
Weekly focus: Reposition professionally + resume training.
Alert: "Career strategy" goal has no next action defined.
```

---

## 2. Weekly Planning Flow

**When:** Sunday evening or Monday morning.
**Duration target:** 10-15 minutes.

### Steps

1. **Review last week's review** (if exists) — show wins, frictions, focus
2. **Show current CycleFocus** — what are the 2-3 priority fronts?
3. **Show active P1 goals** — status, next actions, staleness
4. **Prompt:** "What are your top 3 priorities this week?"
5. **Prompt:** "What is your minimum victory this week?"
6. **Prompt:** "What will you NOT do this week?"
7. **Show upcoming ReviewDates** for goals/decisions

### Output

- Saved as part of the weekly cadence (not a formal entity — just displayed from goals + cycle focus)
- User can optionally create/update tasks

---

## 3. Weekly Review Flow

**When:** Friday evening or Sunday.
**Duration target:** 10-15 minutes.

### Input

User fills:
1. **Wins** — what went well (free text)
2. **Frictions** — what was hard (free text)
3. **Avoided things** — what was dodged (free text)
4. **Energy drains** — what consumed disproportionate energy
5. **Energy gains** — what generated energy
6. **Main insight** — one key learning
7. **Next week focus** — primary direction
8. **Pillar scores** — quick 1-10 re-assessment per pillar (optional)

### Processing

System saves `WeeklyReview` entity.
If pillar scores changed, update Pillar.Score and set LastReviewedAt.
System checks:
- Any P1 goals without progress? → Flag
- Any pillars declining for 3+ weeks? → Alert
- Review count < 2 in last month? → Nudge

### Output

- Saved review accessible in history
- Dashboard updated with fresh pillar scores
- Alerts generated if applicable

---

## 4. Goal Creation Flow

**When:** User decides to create a new front/objective.

### Input (minimal required)

1. Title
2. Pillar (select)
3. Horizon (select enum)
4. Status (default: Idea)
5. Priority (default: P3)

### Input (optional, progressive)

6. Description
7. Why it matters
8. Success criteria
9. Next action
10. Main risk
11. Target date
12. Review date

### Validation

- If Priority = P1 and Status = Active: check if < 3 P1 Active goals exist
  - If 3 already exist: warn "You already have 3 P1 fronts. Demote one before adding another."
- If Status = Active and NextAction is empty: warn "Active goals work better with a defined next action."

### Output

- Goal saved
- Appears in dashboard if P1/Active
- Linked to pillar

---

## 5. Decision Registration Flow

**When:** User wants to consolidate a strategic choice.

### Input (minimal required)

1. Title
2. Context — what prompted this decision

### Input (optional, progressive)

3. Problem statement
4. Options considered (structured list or free text)
5. Chosen option
6. Rationale
7. Trade-offs accepted
8. Risks accepted
9. Pillar link
10. Goal link
11. Review date

### Processing

- Save Decision entity
- If linked to a Goal, the decision appears in the goal detail view

### Output

- Decision accessible in Decisions list and linked entities
- If ReviewDate set, appears in upcoming reviews

---

## 6. Journal Entry Flow

**When:** User wants to capture a thought, insight, observation, or reflection.

### Input (minimal required)

1. Content (free text)
2. Entry type (select enum — default: Reflection)

### Input (optional)

3. Title
4. Pillar link
5. Goal link
6. Decision link
7. Tags
8. Mood (1-10)
9. Energy (1-10)
10. Intensity (1-10)
11. Is sensitive (checkbox)

### Processing

- Save JournalEntry
- If tags or content match an existing Pattern name, suggest linking

### Output

- Entry saved and searchable
- Appears in pillar/goal detail if linked

---

## 7. Cycle Focus Flow

**When:** User defines or updates their 30-90 day focus.

### Input

1. Title (e.g., "Q2 2026: Base Rebuild")
2. Thesis (one paragraph: what this cycle is about)
3. Start date
4. End date
5. Link 2-3 goals as the cycle's priority fronts

### Validation

- Only one CycleFocus can be active at a time
- Warn if more than 3 goals are linked

### Output

- CycleFocus visible on Dashboard
- Linked goals get visual priority

---

## 8. Search Flow

**When:** User looks for anything.

### Searchable entities

- Goals (title, description, next action)
- Decisions (title, context, chosen option)
- Journal entries (title, content)
- Patterns (name, description)
- Weekly reviews (all text fields)
- Tasks (title, description)

### Filters

- By entity type
- By pillar
- By date range
- By status
- By tags
- By entry type (journal)
- Sensitive only (toggle)

### Output

- Results list with entity type indicator, date, and snippet
- Click navigates to detail

---

## 9. Dashboard Alerts (v1 — rule-based)

| # | Alert | Trigger | Priority |
|---|-------|---------|----------|
| A1 | Too many P1 fronts | >3 P1 Active goals | High |
| A2 | Goal without next action | Active goal, NextAction empty | Medium |
| A3 | Stale goal | Active goal, no update in 21+ days | Medium |
| A4 | Neglected pillar | Pillar not reviewed in 30+ days | Medium |
| A5 | Low energy streak | 3+ consecutive check-ins with Energy ≤ 4 | High |
| A6 | Missing weekly review | No review for current week by Sunday | Low |
| A7 | Decision needs review | Decision.ReviewDate passed | Medium |
| A8 | Overloaded day | User sets >5 tasks as "Doing" | Low |

---

## 10. Export & Backup Flow

### Export formats

| Format | Content |
|--------|---------|
| JSON | Full database export, all entities |
| Markdown | Journal entries and decisions as .md files |
| CSV | Metrics and check-in data |

### Backup

- Manual trigger: user clicks "Backup Now"
- Creates timestamped copy of SQLite database file
- Stored in user-chosen directory
- System tracks last backup date and warns if >7 days old
