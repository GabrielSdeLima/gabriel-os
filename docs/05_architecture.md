# Gabriel OS — Architecture

## 1. Stack

| Layer | Technology | Rationale |
|-------|-----------|-----------|
| Runtime | .NET 8 (LTS) | Long-term support, mature ecosystem |
| UI Framework | WPF | Battle-tested for Windows desktop, rich MVVM support, large community |
| MVVM Toolkit | CommunityToolkit.Mvvm | Source generators, minimal boilerplate, Microsoft-maintained |
| Database | SQLite | Zero-config, single-file, perfect for offline-first personal app |
| ORM | EF Core 8 + SQLite provider | Migrations, typed queries, clean abstractions |
| UI Theme | MaterialDesignInXAML (optional) | Modern look with minimal effort; can be added later |
| Logging | Serilog (file sink) | Simple, structured, local-only |
| Testing | xUnit | Standard, when applicable |
| Version Control | Git + GitHub | |

## 2. Architecture Pattern

Clean Architecture with 4 projects:

```
GabrielOS.sln
├── src/
│   ├── GabrielOS.Domain/           # Entities, enums, interfaces, business rules
│   ├── GabrielOS.Application/      # Use cases, services, DTOs, validation
│   ├── GabrielOS.Infrastructure/   # EF Core, SQLite, repositories, file export
│   └── GabrielOS.Presentation/     # WPF app, ViewModels, Views, navigation
├── tests/
│   └── GabrielOS.Tests/            # Unit tests (added as needed)
└── docs/                           # Product documentation
```

### Dependency Flow

```
Presentation → Application → Domain
                    ↓
              Infrastructure → Domain
```

- **Domain** has zero dependencies. Pure C# classes.
- **Application** depends only on Domain. Contains interfaces that Infrastructure implements.
- **Infrastructure** implements persistence, export, and external concerns.
- **Presentation** depends on Application (and transitively Domain). Contains all UI.

### Key Rules

1. Domain entities have no EF Core attributes. Configuration is in Infrastructure via Fluent API.
2. ViewModels never touch DbContext directly. They go through Application services.
3. Navigation is managed by a central NavigationService in Presentation.
4. All DI registration happens in Presentation's startup.

## 3. Project Details

### GabrielOS.Domain

```
Domain/
├── Entities/
│   ├── User.cs
│   ├── Pillar.cs
│   ├── Goal.cs
│   ├── Initiative.cs
│   ├── Decision.cs
│   ├── CheckIn.cs
│   ├── JournalEntry.cs
│   ├── WeeklyReview.cs
│   ├── CycleFocus.cs
│   ├── CycleFocusGoal.cs
│   ├── Pattern.cs
│   ├── Metric.cs
│   └── Task.cs
├── Enums/
│   ├── PillarPriority.cs
│   ├── Trend.cs
│   ├── HorizonType.cs
│   ├── GoalStatus.cs
│   ├── GoalPriority.cs
│   ├── DecisionStatus.cs
│   ├── SuggestedMode.cs
│   ├── EntryType.cs
│   ├── PatternStatus.cs
│   └── TaskStatus.cs
└── Interfaces/
    ├── IRepository.cs          # Generic CRUD
    ├── IPillarRepository.cs
    ├── IGoalRepository.cs
    ├── ICheckInRepository.cs
    └── ...
```

### GabrielOS.Application

```
Application/
├── Services/
│   ├── PillarService.cs
│   ├── GoalService.cs
│   ├── CheckInService.cs
│   ├── JournalService.cs
│   ├── DecisionService.cs
│   ├── ReviewService.cs
│   ├── CycleFocusService.cs
│   ├── SearchService.cs
│   ├── ExportService.cs
│   └── AlertService.cs
├── DTOs/
│   └── ...
└── Rules/
    ├── ModeCalculator.cs       # Check-in → SuggestedMode
    └── AlertRules.cs           # Dashboard alert logic
```

### GabrielOS.Infrastructure

```
Infrastructure/
├── Data/
│   ├── AppDbContext.cs
│   ├── Configurations/         # EF Fluent API per entity
│   │   ├── PillarConfiguration.cs
│   │   ├── GoalConfiguration.cs
│   │   └── ...
│   └── Migrations/
├── Repositories/
│   ├── PillarRepository.cs
│   ├── GoalRepository.cs
│   └── ...
├── Export/
│   ├── JsonExporter.cs
│   ├── MarkdownExporter.cs
│   └── BackupService.cs
└── Seeding/
    └── DefaultDataSeeder.cs    # Default pillars on first run
```

### GabrielOS.Presentation

```
Presentation/
├── App.xaml / App.xaml.cs      # Startup, DI container
├── MainWindow.xaml             # Shell with navigation
├── Navigation/
│   ├── INavigationService.cs
│   └── NavigationService.cs
├── ViewModels/
│   ├── DashboardViewModel.cs
│   ├── PillarListViewModel.cs
│   ├── PillarDetailViewModel.cs
│   ├── CheckInViewModel.cs
│   ├── JournalListViewModel.cs
│   ├── JournalEntryViewModel.cs
│   └── ...
├── Views/
│   ├── DashboardView.xaml
│   ├── PillarListView.xaml
│   ├── PillarDetailView.xaml
│   ├── CheckInView.xaml
│   ├── JournalListView.xaml
│   ├── JournalEntryView.xaml
│   └── ...
├── Converters/                 # Value converters for XAML
├── Resources/
│   ├── Colors.xaml
│   ├── Fonts.xaml
│   └── Styles.xaml
└── Controls/                   # Reusable custom controls
```

## 4. Database Strategy

### Location
- Default: `%LOCALAPPDATA%/GabrielOS/gabrielos.db`
- Configurable in settings

### Migrations
- EF Core Code-First migrations
- Applied automatically on app startup
- Migration history tracked in DB

### Backup
- Copy SQLite file to user-specified directory
- Timestamped filename: `gabrielos_backup_20260326_1430.db`
- Last backup date tracked; warn if > 7 days old

### Seeding
- On first run (empty DB), create default User and default Pillars
- Seed data defined in `DefaultDataSeeder.cs`

## 5. Navigation Model

Single-window app with sidebar navigation.

### Navigation targets (MVP-0)
- Dashboard
- Pillars (list → detail)
- Check-in
- Journal (list → entry)

### Navigation targets (MVP-1, added)
- Goals (list → detail)
- Decisions (list → detail)
- Weekly Review
- Cycle Focus
- Search
- Settings

### Navigation implementation
- Frame-based navigation in WPF
- ViewModels resolved via DI
- DataTemplate mapping ViewModel → View

## 6. Design System (Minimal v1)

### Colors
```
Background:     #1E1E2E (dark) or #FAFAFA (light)
Surface:        #2A2A3C (dark) or #FFFFFF (light)
Primary:        #7C9EBD
Primary Dark:   #5A7D9A
Accent:         #A3BE8C
Text Primary:   #D8DEE9 (dark) or #2E3440 (light)
Text Secondary: #808A9F (dark) or #6B7280 (light)
Danger:         #BF616A
Warning:        #EBCB8B
Success:        #A3BE8C
```

### Typography
```
Font:           Segoe UI (system default for Windows)
Headings:       Segoe UI Semibold
Body:           14px
Small:          12px
H1:             24px Semibold
H2:             18px Semibold
H3:             16px Semibold
```

### Components
- Cards for entities (pillars, goals, decisions)
- Sidebar navigation with icons
- Slider inputs for 1-10 scales
- Enum selectors as segmented buttons or dropdowns
- Modal dialogs for confirmations
- Toast notifications for alerts

## 7. Future Considerations (Not Now, But Prepared For)

### AI Integration (v2+)
- Application layer will have `IAIService` interface
- Infrastructure will implement it with HTTP calls to Anthropic/OpenAI API
- AI never touches Domain directly — it operates through Application services
- Context assembly will be a dedicated pipeline in Application

### Encryption (v3+)
- Sensitive entries flagged in domain model
- Infrastructure can implement per-field encryption when needed
- SQLite Cipher as alternative for full-DB encryption

### Cross-platform (v3+)
- Clean Architecture makes it possible to swap Presentation layer
- Avalonia or MAUI could replace WPF later
- Domain + Application + Infrastructure remain unchanged
