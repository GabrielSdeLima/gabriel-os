# Gabriel OS — MVP Scope

## 1. Two-Tier MVP Strategy

Building the full MVP in one shot is too risky for a solo builder. Instead, we split into:

- **MVP-0 (2 weeks):** Minimum usable daily system — something you open every day while building the rest.
- **MVP-1 (6-8 weeks):** Full foundation — all core modules operational.

This ensures you have a working tool from week 2, not week 10.

---

## 2. MVP-0 — Daily Operating Base

**Goal:** A system you open every morning and use every day.

### What's in

| Module | Scope |
|--------|-------|
| Pillars | CRUD, list view, detail view, manual score |
| Check-in | Daily form with mode suggestion |
| Journal | Create entries, list, basic search |
| Dashboard | Minimal — pillar scores, today's check-in, active cycle focus |
| Shell | App opens, navigates between modules, persists data |

### What's out of MVP-0

- Goals module (use journal + cycle focus as lightweight substitute)
- Decisions module
- Weekly review module
- Tasks
- Patterns
- Metrics
- Alerts/recommendations
- Export/backup
- Search with filters

### Definition of Done — MVP-0

- [ ] App opens to a dashboard showing pillar scores and today's state
- [ ] User can create, edit, and view pillars
- [ ] User can do a daily check-in in < 3 minutes
- [ ] Check-in generates a suggested mode
- [ ] User can write and browse journal entries
- [ ] All data persists in SQLite locally
- [ ] App compiles and runs on Windows without external dependencies

---

## 3. MVP-1 — Full Foundation

**Goal:** All core modules operational, system generates real value.

### What's added on top of MVP-0

| Module | Scope |
|--------|-------|
| Goals | Full CRUD, horizons, statuses, pillar link, next action |
| Decisions | Full CRUD, context, options, rationale, review date |
| Weekly Review | Structured form, pillar score snapshot |
| Cycle Focus | Create/edit, link goals, display on dashboard |
| Tasks | Basic CRUD, link to goals, status tracking |
| Patterns | Simple CRUD (name, description, trigger, status) |
| Metrics | Record values per pillar |
| Dashboard v2 | Alerts, cycle focus, P1 goals, stale goal warnings |
| Search | Global search across entities with filters |
| Export/Backup | JSON full export, SQLite backup, Markdown for journal |

### What stays out of MVP-1

- AI agents / LLM integration
- Computed scoring (weighted, algorithmic)
- UI mode adaptation (Protect/Simplify changing layout)
- Monthly review as separate entity
- Timeline / longitudinal visualization
- Integration with calendar, email, or external tools
- Design system beyond basic theme
- Mobile / cross-platform

### Definition of Done — MVP-1

- [ ] All MVP-0 criteria still pass
- [ ] User can manage goals by horizon and priority
- [ ] System enforces max 3 P1 Active goals (with warning)
- [ ] User can register and review decisions
- [ ] User can complete a weekly review in < 15 minutes
- [ ] User can define a cycle focus with linked goals
- [ ] Dashboard shows alerts for stale goals, neglected pillars, overloaded P1s
- [ ] User can search across all entities
- [ ] User can export full backup
- [ ] All business rules from domain model are enforced

---

## 4. Milestones

### Milestone 1 — Foundation (Week 1)
- Solution structure, DI, navigation shell
- Domain entities and EF Core setup
- SQLite database with migrations
- Basic theme and layout

### Milestone 2 — MVP-0 Core (Week 2)
- Pillar CRUD + list/detail views
- Check-in daily flow
- Journal CRUD + list
- Minimal dashboard
- **🎯 MVP-0 complete — start using daily**

### Milestone 3 — Goals & Decisions (Weeks 3-4)
- Goal CRUD with validation rules
- Decision CRUD
- Link goals and decisions to pillars
- Update dashboard with goal info

### Milestone 4 — Reviews & Focus (Weeks 5-6)
- Weekly review form
- Cycle focus CRUD + goal linking
- Dashboard v2 with alerts
- Pillar score history from reviews

### Milestone 5 — Operations & Polish (Weeks 7-8)
- Tasks CRUD
- Patterns CRUD
- Metrics CRUD
- Global search
- Export/backup
- UX review and cleanup
- **🎯 MVP-1 complete**

---

## 5. Out of Scope — Future Versions

### v2 (after MVP-1 is stable and used daily for 4+ weeks)
- Monthly review entity
- Trend charts per pillar
- Pattern detection suggestions
- LLM-powered weekly summary
- Computed scoring
- Keyboard shortcuts and power-user features

### v3+
- Full AI agent layer (Analyst, Strategist, Auditor roles)
- Scenario simulation
- Calendar integration
- Mobile companion (read-only at first)
- Encrypted sensitive entries

---

## 6. Key Risk Mitigations

| Risk | Mitigation |
|------|------------|
| Building too much before using | MVP-0 forces daily use by week 2 |
| Overengineering domain model | Fields are minimal-required; optional fields appear progressively |
| Scoring becomes punitive | Scores are manual 1-10 only; no computed judgment |
| Patterns module too complex | v1 patterns are just tagged notes with trigger field |
| AI distraction | Zero AI in v1; clear separation for v2+ |
| Data loss | SQLite + manual backup from MVP-1; autosave always |
