<!--
  Sync Impact Report
  ==================
  Version change: 1.1.2 → 1.1.3 (PATCH — added Lambda avoidance + Statement formatting to Principle IV)
  Modified sections:
    - Principle IV: Added "Lambda avoidance" and "Statement formatting" rules
  Clarifications:
    - Lambda expressions MUST NOT be used where named methods suffice
    - Each statement MUST occupy its own line (no multi-statement folding)
    - Method calls MUST NOT be broken across lines unless >120 chars
    - Only exception: Prism expression-tree lambdas (ObservesProperty)
  Removed sections: None
  Templates requiring updates: None
  Follow-up TODOs: None
-->

# WPF Demo Project Constitution

## Core Principles

### I. Prism MVVM Architecture

All UI components MUST follow the Model-View-ViewModel (MVVM) architectural pattern
as implemented by the **Prism** framework. Deviation from Prism conventions is
prohibited:

- **View**: XAML-based UI with data binding and Prism region attachments; zero
  code-behind logic beyond `InitializeComponent()` and region registration. Views
  MUST NOT contain business logic, direct data access, or manual ViewModel
  instantiation.
- **ViewModel**: MUST inherit from `Prism.Mvvm.BindableBase` (not manual
  `INotifyPropertyChanged`). Property setters MUST use `SetProperty<T>(ref field,
  value)` for change notification. Commands MUST use `Prism.Commands.DelegateCommand`
  (not `ICommand` manual implementations).
- **Model**: Domain objects and data representations; no UI or Prism framework
  coupling.
- **Navigation**: View-to-View navigation MUST use Prism `IRegionManager` with
  region-based composition. Direct window instantiation via `new Window().Show()`
  is prohibited.
- **Modules**: Multi-window or large features MUST be organized as Prism `IModule`
  implementations, loaded via `IModuleCatalog` or `IModuleManager`.
- **Events**: Cross-ViewModel communication MUST use Prism `IEventAggregator` with
  typed `PubSubEvent<T>` payloads. Raw .NET events between ViewModels are prohibited.

**Rationale**: Prism provides battle-tested MVVM infrastructure that eliminates
boilerplate (BindableBase, DelegateCommand), enforces decoupling (RegionManager,
EventAggregator), and scales to large applications (Modules). Mandating Prism
ensures every developer follows the same patterns, reducing architectural drift
and making code navigation predictable across the entire codebase.

### II. Interface-Oriented Services + Dependency Injection

The service layer MUST be defined through interfaces, and all service instantiation
MUST occur via dependency injection:

- Every service class MUST implement a corresponding interface (e.g.,
  `IDataService` → `DataService`).
- Service lifetimes MUST be explicitly configured in the Prism DI container
  (transient, scoped, or singleton) with documented rationale. Prism uses
  DryIoc as the default container; registration MUST follow DryIoc API conventions
  within `RegisterTypes` in the Prism `App.xaml.cs`.
- ViewModels and services MUST receive dependencies through constructor injection
  only; property injection and service locator patterns are prohibited.
- Prism's `IContainerProvider` or `IContainerRegistry` MUST be the sole mechanism
  for DI registration; direct `new` of services or ViewModels is prohibited.

**Rationale**: Interface-oriented design enables unit testing with mocks/stubs,
supports swapping implementations without consumer changes, and makes dependency
graphs explicit and auditable. Constructor injection guarantees that required
dependencies are never in a null state. Using Prism's built-in DryIoc container
avoids redundant DI abstraction layers.

### III. Unified Style Design (Panuon.WPF.UI)

Visual styling MUST be centralized and built upon **Panuon.WPF.UI** to guarantee
consistent user experience across all modules:

- **Theme Foundation**: The application MUST reference Panuon.WPF.UI's theming
  system. All custom styles MUST derive from Panuon base styles and theme tokens
  unless an explicit design decision requires a standalone style (requires
  architecture review approval).
- **Resource Dictionary**: All project-specific colors, fonts, spacing, and control
  templates MUST be defined in a single `ResourceDictionary` that merges Panuon
  theme resources — never hardcoded in individual XAML views.
- **Resource Keys**: MUST follow the convention `{Category}.{Property}.{Variant}`
  (e.g., `Color.Primary.Background`, `Font.Size.Heading`). Panuon theme resource
  keys MUST be used directly where applicable; do not redefine them.
- **Style Inheritance**: Custom control styles MUST inherit from Panuon base styles
  using `BasedOn="{StaticResource {x:Type ControlType}}"`. Standalone styles
  require architecture review approval.
- **Module Coverage**: Every new module or window MUST reference the shared resource
  dictionary; local overrides are prohibited without documented justification.

**Rationale**: Panuon.WPF.UI provides a production-ready WPF theming infrastructure
with modern controls, light/dark theme support, and consistent design tokens.
Centralizing all styling on Panuon prevents visual drift across modules, eliminates
the need to hand-build control templates, and makes theme-wide changes (e.g.,
dark mode, accent color) a single-location operation. Consistent UX reduces user
cognitive load when navigating between application areas.

### IV. Code Quality — Lean, Readable, Maintainable

All code MUST meet the following non-negotiable quality standards:

- **Readability**: Methods MUST be ≤50 lines; files MUST be ≤800 lines. Nested
  blocks MUST NOT exceed 4 levels — refactor with early returns or extracted methods.
- **Naming**: `camelCase` for locals/parameters, `PascalCase` for types/methods,
  `UPPER_SNAKE_CASE` for constants. Boolean variables MUST use `is`/`has`/`can`
  prefixes.
- **Immutability**: Prefer `readonly` fields and immutable data structures. Mutations
  MUST be explicit, localized, and justified.
- **No dead code**: Commented-out code, unused `using` directives, and unreachable
  branches MUST be removed before merge.
- **DRY**: Duplicated logic beyond 3 lines MUST be extracted into a shared method
  or utility class.
- **Lambda avoidance**: Lambda expressions MUST NOT be used where a named method
  would suffice. Event handlers, command delegates, and factory methods MUST use
  named methods rather than inline lambdas. The only permitted exception is
  expression-tree lambdas required by framework APIs (e.g., `.ObservesProperty()`).
- **Statement formatting**: Each statement MUST occupy its own line. Multiple
  statements MUST NOT be folded onto a single line. Method calls MUST NOT be
  unnecessarily broken across multiple lines unless exceeding 120 characters.
  Single-statement `if`/`else` bodies MAY omit braces.
- **Comment coverage**: Code comment rate MUST be ≥80%. Every public class,
  interface, method, and property MUST have an XML documentation comment
  (`/// <summary>`). Complex logic branches and non-obvious design decisions
  MUST have inline explanatory comments. Only trivial getters/setters and
  constructors that purely assign dependencies are exempt.

**Rationale**: Code is read far more often than written. Named methods are
self-documenting and searchable; lambdas obscure intent in call stacks and
debugging. One-statement-per-line prevents visual clutter and makes diffs
precise. High comment coverage ensures new team members can understand code
without reverse-engineering intent.

### V. Performance & Testing

Performance efficiency and test coverage are mandatory for every feature:

- **Memory**: ViewModels MUST implement `IDisposable` where they subscribe to
  long-lived events (including Prism `IEventAggregator` subscriptions). Event
  handlers MUST be unsubscribed to prevent memory leaks. Data templates MUST use
  virtualization (`VirtualizingStackPanel`) for any collection with potentially
  >100 items.
- **Unit Tests**: Every public method, property, and command in ViewModels and
  services MUST have a corresponding unit test. Minimum line coverage threshold
  is 80%.
- **Test Structure**: Tests MUST follow the Arrange-Act-Assert (AAA) pattern.
  Test names MUST describe behavior, not implementation (e.g.,
  `CalculateTotal_ReturnsZero_WhenCartIsEmpty`).
- **Test Isolation**: Unit tests MUST NOT depend on file system, network, database,
  or Prism container initialization. Use mocking frameworks (Moq/NSubstitute) for
  all external dependencies including Prism services (`IRegionManager`,
  `IEventAggregator`, etc.).
- **Regression**: Any bug fix MUST include a failing test reproducing the bug
  before the fix is applied.

**Rationale**: Low memory usage is critical for desktop applications running
alongside other user workloads. Mandatory unit tests provide a safety net for
refactoring, serve as living documentation, and catch regressions before they
reach users.

## Technical Standards

### Technology Stack

| Layer | Technology | Notes |
|-------|-----------|-------|
| UI Framework | WPF (.NET 8+) | Windows Presentation Foundation |
| MVVM Framework | Prism.Wpf (8.x) | Prism MVVM with DryIoc container |
| UI Component Library | Panuon.WPF.UI | Modern controls + theming system |
| DI Container | Prism DryIoc (built-in) | Ships with Prism.Wpf |
| Testing Framework | xUnit | Primary test framework |
| Mocking | Moq or NSubstitute | Chosen at project initialization |
| Logging | Microsoft.Extensions.Logging | Structured logging |

### Project Type Requirements

All projects MUST reside within a **single** Visual Studio solution (`.sln`) file.
The solution MUST contain **exactly one** WPF Application project — the startup/Shell.
All other module projects MUST be WPF Class Library projects. No module may be a
WPF Application or executable project.

| Project Role | SDK / Type | Notes |
|-------------|-----------|-------|
| **Startup / Shell** (ONLY ONE) | WPF Application (`<UseWPF>true</UseWPF>`) | Sole entry point: `App.xaml` + `MainWindow.xaml`; Prism bootstrapper; the ONLY executable project |
| **Module projects** (all others) | **WPF Class Library** (`<UseWPF>true</UseWPF>`) | Contains XAML Views; MUST use WPF SDK; MUST NOT be executable; loaded via Prism `IModuleCatalog` |
| **Core library** | .NET Class Library | Models, events, enums only; no WPF or UI dependencies |
| **Services library** | .NET Class Library | Interfaces + implementations; no WPF or UI dependencies |
| **Test projects** | xUnit Test Project | One test project per module + one per service library |

Each Prism module (Login, UserManagement, TodoList, etc.) MUST be a **WPF class library**
project (`Microsoft.NET.Sdk` with `<UseWPF>true</UseWPF>`), not a plain .NET class library,
because modules contain XAML Views, ResourceDictionaries, and WPF converters. A plain class
library cannot compile XAML files.

### Code Organization

```text
WpfTodoApp.sln                           # Single solution containing all projects
│
├── src/WpfTodoApp/                      # WPF Application (startup project)
│   ├── App.xaml                         # PrismApplication entry point
│   ├── App.xaml.cs                      # DI composition root + module catalog
│   ├── MainWindow.xaml                  # Shell window + Prism Regions
│   ├── MainWindow.xaml.cs
│   ├── Resources/
│   │   └── AppResources.xaml            # Panuon theme merge + custom styles
│   └── WpfTodoApp.csproj                # Sdk="Microsoft.NET.Sdk" + UseWPF=true
│
├── src/WpfTodoApp.Core/                 # .NET Class Library (no WPF)
│   ├── Models/
│   │   ├── User.cs
│   │   └── TodoItem.cs
│   ├── Events/
│   │   ├── UserLoggedInEvent.cs
│   │   └── TodoDueReminderEvent.cs
│   └── WpfTodoApp.Core.csproj           # Sdk="Microsoft.NET.Sdk" (plain)
│
├── src/WpfTodoApp.Services/             # .NET Class Library (no WPF)
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IUserService.cs
│   │   ├── ITodoService.cs
│   │   └── IReminderService.cs
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── AuthService.cs
│   ├── UserService.cs
│   ├── TodoService.cs
│   ├── ReminderService.cs
│   └── WpfTodoApp.Services.csproj       # Sdk="Microsoft.NET.Sdk" (plain)
│
├── src/WpfTodoApp.Modules.Login/        # WPF Class Library (module)
│   ├── Views/
│   │   ├── LoginView.xaml
│   │   └── ChangePasswordView.xaml
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   └── ChangePasswordViewModel.cs
│   ├── LoginModule.cs
│   └── WpfTodoApp.Modules.Login.csproj  # Sdk="Microsoft.NET.Sdk" + UseWPF=true
│
├── src/WpfTodoApp.Modules.UserManagement/  # WPF Class Library (module)
│   ├── Views/
│   │   └── UserManagementView.xaml
│   ├── ViewModels/
│   │   └── UserManagementViewModel.cs
│   ├── UserManagementModule.cs
│   └── WpfTodoApp.Modules.UserManagement.csproj  # + UseWPF=true
│
├── src/WpfTodoApp.Modules.TodoList/     # WPF Class Library (module)
│   ├── Views/
│   │   ├── TodoListView.xaml
│   │   └── TodoEditView.xaml
│   ├── ViewModels/
│   │   ├── TodoListViewModel.cs
│   │   └── TodoEditViewModel.cs
│   ├── Converters/
│   │   ├── PriorityToColorConverter.cs
│   │   └── BoolToStatusConverter.cs
│   ├── TodoListModule.cs
│   └── WpfTodoApp.Modules.TodoList.csproj  # + UseWPF=true
│
tests/
├── WpfTodoApp.Core.Tests/               # xUnit Test Project
├── WpfTodoApp.Services.Tests/           # xUnit Test Project
├── WpfTodoApp.Modules.Login.Tests/      # xUnit Test Project
├── WpfTodoApp.Modules.UserManagement.Tests/  # xUnit Test Project
├── WpfTodoApp.Modules.TodoList.Tests/   # xUnit Test Project
└── WpfTodoApp.Integration.Tests/        # xUnit Test Project (optional)
```

**Key constraints**:
- `WpfTodoApp` is the **one and only** startup/WPF Application project in the entire solution.
- All module projects (`WpfTodoApp.Modules.*`) MUST be WPF class library projects —
  they MUST NOT be executable WPF Application projects.
- Module projects are loaded by the startup project via Prism `IModuleCatalog`; they
  cannot run independently.

### Prism Bootstrapper (Mandatory Pattern)

All Prism application initialization MUST follow this pattern in `App.xaml.cs`:

```csharp
public partial class App : PrismApplication
{
    protected override Window CreateShell()
    {
        return Container.Resolve<MainWindow>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        // Services
        containerRegistry.Register<IDataService, DataService>();
        // Navigation
        containerRegistry.RegisterForNavigation<MainView, MainViewModel>();
    }

    protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
        // Register Prism modules
    }
}
```

## Development Workflow

### Feature Implementation Flow

1. **Spec** — Write or update the feature spec with user stories and acceptance criteria.
2. **Plan** — Create implementation plan covering architecture, data model, and
   file-level task breakdown.
3. **Test-First** — Write unit tests FIRST for ViewModels and services; verify
   they FAIL before implementation.
4. **Implement** — Code to green tests, adhering to Principles I–V.
5. **Review** — Self-review against this constitution; address all deviations
   before marking complete.
6. **Verify** — Run full test suite; confirm ≥80% coverage; perform manual smoke
   test of the feature.

### Quality Gates

- All unit tests pass with ≥80% line coverage.
- No Prism MVVM violations:
  - All ViewModels inherit from `BindableBase`.
  - All properties use `SetProperty<T>()`.
  - All commands use `DelegateCommand`.
  - Navigation goes through `IRegionManager`.
  - Cross-ViewModel communication uses `IEventAggregator`.
- No Panuon.WPF.UI violations:
  - No hand-rolled control templates that duplicate Panuon styles.
  - All custom styles derive from Panuon base styles via `BasedOn`.
- No hardcoded styles in individual XAML files.
- All services and ViewModels registered via Prism `IContainerRegistry`.
- No memory-leak patterns (unsubscribed EventAggregator subscriptions, undisposed
  resources).

### Architecture Compliance

Architecture violations are treated as **BLOCKING** issues. The following are
zero-tolerance violations:

| Violation | Detection | Severity |
|-----------|-----------|----------|
| ViewModel not inheriting `BindableBase` | Code review / static analysis | CRITICAL |
| Manual `ICommand` instead of `DelegateCommand` | Code review | HIGH |
| Direct `new Window().Show()` instead of Prism navigation | Code review | CRITICAL |
| Service locator pattern (`ServiceLocator.Current`) | Code review | CRITICAL |
| Raw .NET events between ViewModels | Code review | HIGH |
| Panuon.WPF.UI not referenced in new XAML View | Code review | HIGH |
| Hand-rolled control templates duplicating Panuon | Code review | HIGH |
| Hardcoded styles, colors, or fonts in XAML | Code review | HIGH |

Any CRITICAL violation discovered post-merge MUST be reverted or hotfixed within
24 hours. HIGH violations MUST be remediated within the current sprint.

## Governance

This constitution is the supreme authority for the WPF Demo project. All code
contributions, architecture decisions, and design choices MUST comply with these
principles. When a conflict arises between this constitution and any other guidance
document, the constitution prevails.

### Amendment Process

1. Propose the amendment with rationale in a feature spec or dedicated discussion.
2. Review the amendment for consistency with existing principles.
3. Update `.specify/memory/constitution.md` with version bump per semantic
   versioning rules (MAJOR for principle removal/redefinition, MINOR for new
   principles or materially expanded guidance, PATCH for clarifications).
4. Propagate changes to dependent templates and documentation.
5. Communicate the amendment to the development team.

### Compliance Review

- Every implementation plan MUST include a **Constitution Check** section
  verifying alignment with each principle before Phase 0 research.
- Every code review MUST verify compliance with Principles I–V, using the
  Architecture Compliance table as a checklist.
- Non-compliance discovered post-merge MUST be tracked as a HIGH or CRITICAL
  priority issue per the Architecture Compliance severity matrix.

### Versioning Policy

```text
MAJOR.MINOR.PATCH
  MAJOR — Backward-incompatible governance or principle removal/redefinition
  MINOR — New principle or section added; materially expanded guidance
  PATCH — Clarifications, wording fixes, non-semantic refinements
```

**Version**: 1.1.4 | **Ratified**: 2026-06-08 | **Last Amended**: 2026-06-09
