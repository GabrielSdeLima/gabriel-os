# Gabriel OS

A personal life operating system for Windows desktop.

Transforms reflection into direction, direction into execution, and execution into learning.

## What It Does

- **Pillars:** Organize life into core domains with vision, state, and score
- **Check-in:** Daily state snapshot with intelligent mode suggestion
- **Journal:** Structured capture of insights, patterns, and reflections
- **Goals:** Manage objectives by horizon, priority, and status
- **Decisions:** Consolidate strategic choices and stop mental loops
- **Reviews:** Weekly retrospectives with pillar score tracking
- **Cycle Focus:** Define 2-3 priority fronts for 30-90 day periods
- **Dashboard:** Cockpit view with alerts and recommendations

## Stack

- .NET 8 / C# / WPF / MVVM
- SQLite (offline-first, local only)
- CommunityToolkit.Mvvm
- Clean Architecture (Domain → Application → Infrastructure → Presentation)

## Project Structure

```
GabrielOS.sln
├── src/
│   ├── GabrielOS.Domain/           # Entities, enums, interfaces
│   ├── GabrielOS.Application/      # Services, DTOs, rules
│   ├── GabrielOS.Infrastructure/   # EF Core, SQLite, repositories
│   └── GabrielOS.Presentation/     # WPF app, ViewModels, Views
├── tests/
│   └── GabrielOS.Tests/
└── docs/                           # Product documentation
```

## Documentation

| Document | Description |
|----------|-------------|
| [Product Vision](docs/01_product_vision.md) | What the product is, principles, non-goals |
| [Domain Model](docs/02_domain_model.md) | Entities, relationships, business rules |
| [Core Flows](docs/03_core_flows.md) | User flows, validation, alert logic |
| [MVP Scope](docs/04_mvp_scope.md) | Two-tier MVP strategy, milestones |
| [Architecture](docs/05_architecture.md) | Stack decisions, project structure, design system |
| [AGENT.md](AGENT.md) | Instructions for AI coding agents |

## Development

### Prerequisites
- .NET 8 SDK
- Visual Studio 2022 or VS Code with C# extension
- Git

### Build & Run
```bash
dotnet build
dotnet run --project src/GabrielOS.Presentation
```

### Database
SQLite database created automatically on first run at:
`%LOCALAPPDATA%/GabrielOS/gabrielos.db`

## Current Phase

**MVP-0** — Foundation + Daily Operating Base (Pillars, Check-in, Journal, Dashboard)

## License

Private — personal project.
