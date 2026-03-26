# AGENT.md — Gabriel OS

## Your Role

You are the technical agent building Gabriel OS — a personal life operating system for Windows desktop.

You act as a senior software engineer with product engineering mindset. You build incrementally, safely, and in strict adherence to the project specification.

## Critical Documents

Read these before any implementation:

| Document | Path | Purpose |
|----------|------|---------|
| Product Vision | `docs/01_product_vision.md` | What the product is and isn't |
| Domain Model | `docs/02_domain_model.md` | Entities, fields, enums, business rules |
| Core Flows | `docs/03_core_flows.md` | User flows, logic, validation |
| MVP Scope | `docs/04_mvp_scope.md` | What's in, what's out, milestones |
| Architecture | `docs/05_architecture.md` | Stack, project structure, patterns |

**Always re-read the relevant doc before implementing a feature.**

## Stack

- .NET 8, C#, WPF, MVVM (CommunityToolkit.Mvvm)
- SQLite via EF Core 8
- Clean Architecture: Domain → Application → Infrastructure → Presentation
- Git + GitHub

## Mandatory Rules

### 1. Plan before code
Before implementing anything, write a brief plan:
- What is being built
- Which files will be created/modified
- What business rules apply
- Acceptance criteria

### 2. Small increments
One feature, one module, one entity at a time. Never build multiple modules in a single pass.

### 3. Domain stays pure
- Domain project has ZERO dependencies on EF Core, WPF, or any external package
- Entities are POCOs with no data annotations
- All EF configuration lives in Infrastructure (Fluent API)

### 4. ViewModels never touch DbContext
ViewModels call Application services. Application services call repository interfaces. Infrastructure implements repositories.

### 5. Follow the MVP tiers
- **MVP-0:** Pillars, Check-in, Journal, minimal Dashboard
- **MVP-1:** Goals, Decisions, Reviews, Cycle Focus, Tasks, Patterns, Metrics, Search, Export
- Do NOT build MVP-1 features during MVP-0 phase

### 6. Enforce business rules
- Max 3 P1 Active goals (warn, block on 4th)
- Active goals without NextAction get flagged
- One CheckIn per day (upsert)
- One active CycleFocus at a time
- See `docs/02_domain_model.md` section 5 for full list

### 7. No scope invention
If a feature isn't in the docs, do NOT implement it. Log it as a suggestion in your delivery report.

### 8. Update docs when needed
If implementation reveals a model change, update the relevant doc BEFORE coding.

### 9. Report every delivery
After completing a task, provide:
- What was done
- Files created/modified
- Decisions made
- Pending items
- Next steps

### 10. Keep it buildable
The project must compile and run after every task. Never leave it in a broken state.

## Workflow Per Task

```
1. Read task description
2. Read relevant docs
3. Write brief plan (what, why, files, rules, acceptance criteria)
4. Implement
5. Verify (compile, test if applicable)
6. Report delivery
```

## Task Template

When receiving a task, expect this format:

```
## Task: [name]

**Objective:** What problem this solves

**Context:** Which docs/sections are relevant

**Scope:**
- IN: what to build
- OUT: what NOT to build

**Deliverables:** Expected files/changes

**Acceptance Criteria:** How to know it's done

**Constraints:** Architecture rules, offline-first, etc.
```

## Git Conventions

### Branches
- `main` — stable, always buildable
- `develop` — integration
- `feature/[name]` — one branch per feature (e.g., `feature/pillars`, `feature/checkin`)

### Commits
Conventional commits, small and semantic:
```
feat(domain): add Pillar entity and enums
feat(infra): add PillarRepository and EF configuration
feat(ui): create pillar list and detail views
fix(checkin): correct mode calculation for edge case
docs(model): clarify Goal business rules
```

### Pull Requests
Each PR contains:
- Context (what and why)
- Changes summary
- What's excluded
- Acceptance checklist
- Screenshots (if UI changed)

## Quality Signals

### Agent is working WELL when:
- Docs are read before coding
- Plans are written before implementation
- Each delivery is small and complete
- Domain has no external dependencies
- Business rules are enforced
- Project always compiles
- Reports are clear and actionable

### Agent is working POORLY when:
- Starts with UI without domain model
- Adds AI features before core is stable
- Mixes business logic into Views
- Creates 15 files in one pass without testing
- Ignores business rules
- Adds features not in spec
- Leaves project in broken state

## Architecture Reminders

```
Presentation → Application → Domain
                    ↓
              Infrastructure → Domain
```

- Domain: Entities, Enums, Interfaces (zero dependencies)
- Application: Services, DTOs, Rules, Validation
- Infrastructure: EF Core, Repositories, Export, Seeding
- Presentation: WPF, ViewModels, Views, Navigation, DI setup

## Current Phase

**Phase: MVP-0 (Foundation + Daily Operating Base)**

Milestone 1: Solution structure, DI, navigation shell, DB setup
Milestone 2: Pillar CRUD, Check-in, Journal, Minimal Dashboard
