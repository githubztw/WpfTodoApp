# Tasks: WPF 多模块任务管理系统

**Input**: Design documents from `/specs/001-wpf-task-management/`

**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/

**Tests**: Tests are MANDATORY per constitution Principle V. Every ViewModel and Service must have
corresponding unit tests with ≥80% line coverage. Test tasks are included in every user story phase
and MUST be written FIRST (fail → implement → pass).

**Organization**: Tasks are grouped by user story to enable independent implementation and testing
of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3, US4)
- Include exact file paths in descriptions

## Path Conventions

Based on plan.md project structure. All `src/` and `tests/` paths are relative to repository root.

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization, solution structure, NuGet packages

- [ ] T001 Create Visual Studio solution `WpfTodoApp.sln` at repository root
- [ ] T002 [P] Create `src/WpfTodoApp.Core/WpfTodoApp.Core.csproj` — .NET 8 class library
- [ ] T003 [P] Create `src/WpfTodoApp.Services/WpfTodoApp.Services.csproj` — .NET 8 class library with NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite`, `BCrypt.Net-Next`
- [ ] T004 [P] Create `src/WpfTodoApp/WpfTodoApp.csproj` — .NET 8 WPF application with NuGet packages: `Prism.Wpf` (8.x), `Panuon.WPF.UI`, `Microsoft.Extensions.Logging`
- [ ] T005 [P] Create `src/WpfTodoApp.Modules.Login/WpfTodoApp.Modules.Login.csproj` — .NET 8 WPF class library referencing Core + Services
- [ ] T006 [P] Create `src/WpfTodoApp.Modules.UserManagement/WpfTodoApp.Modules.UserManagement.csproj` — .NET 8 WPF class library referencing Core + Services
- [ ] T007 [P] Create `src/WpfTodoApp.Modules.TodoList/WpfTodoApp.Modules.TodoList.csproj` — .NET 8 WPF class library referencing Core + Services
- [ ] T008 [P] Create test projects: `tests/WpfTodoApp.Services.Tests/`, `tests/WpfTodoApp.Modules.Login.Tests/`, `tests/WpfTodoApp.Modules.UserManagement.Tests/`, `tests/WpfTodoApp.Modules.TodoList.Tests/` — all referencing xUnit + Moq
- [ ] T009 Add solution file referencing all 7 src projects and 4 test projects

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T010 Create `src/WpfTodoApp.Core/Models/User.cs` — entity with properties: `Id` (int, PK), `Username` (string), `PasswordHash` (string), `IsAdmin` (bool), `IsFirstLogin` (bool)
- [ ] T011 [P] Create `src/WpfTodoApp.Core/Models/TodoItem.cs` — entity with properties: `Id` (int, PK), `Title` (string), `Description` (string?), `CreatedAt` (DateTime), `DueDate` (DateTime?), `Priority` (TodoPriority enum), `IsCompleted` (bool), `UserId` (int, FK)
- [ ] T012 [P] Create `src/WpfTodoApp.Core/Models/TodoPriority.cs` — enum: Low=0, Medium=1, High=2
- [ ] T013 [P] Create `src/WpfTodoApp.Core/Events/UserLoggedInEvent.cs` — Prism PubSubEvent with payload: `UserId` (int), `Username` (string)
- [ ] T014 [P] Create `src/WpfTodoApp.Core/Events/TodoDueReminderEvent.cs` — Prism PubSubEvent with payload: `TodoId` (int), `Title` (string), `DueDate` (DateTime)
- [ ] T015 Create `src/WpfTodoApp.Services/Data/AppDbContext.cs` — EF Core DbContext with `DbSet<User>`, `DbSet<TodoItem>`, `OnModelCreating` for schema + seed data (admin user with bcrypt hash of "123456")
- [ ] T016 Create `src/WpfTodoApp/Resources/AppResources.xaml` — merged ResourceDictionary importing Panuon.WPF.UI themes + custom color/font tokens following naming convention `{Category}.{Property}.{Variant}`
- [ ] T017 Create `src/WpfTodoApp/App.xaml` — PrismApplication: override `CreateShell()`, `RegisterTypes(IContainerRegistry)`, `ConfigureModuleCatalog(IModuleCatalog)`; register Core, Services, load AppResources.xaml
- [ ] T018 Create `src/WpfTodoApp/App.xaml.cs` — Prism bootstrapper code-behind per constitution mandatory pattern, register modules (LoginModule, UserManagementModule, TodoListModule) in `ConfigureModuleCatalog`
- [ ] T019 Create `src/WpfTodoApp/MainWindow.xaml` — Shell window with Panuon.WPF.UI Window base style, define Prism regions: `ContentRegion` (main content), `MenuRegion` (navigation menu)

**Checkpoint**: Foundation ready — user story implementation can now begin. All projects build successfully.

---

## Phase 3: User Story 1 - 用户登录与首次密码修改 (Priority: P1) 🎯 MVP

**Goal**: 实现登录界面和首次登录密码修改功能。默认 admin/123456 登录后强制修改密码。

**Independent Test**: 启动应用 → 输入 admin/123456 → 被引导至密码修改界面 → 修改成功进入主界面 → 退出 → 新密码登录成功 → 旧密码登录失败

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T020 [P] [US1] Write unit tests for `AuthService` in `tests/WpfTodoApp.Services.Tests/AuthServiceTests.cs` — test Authenticate with valid credentials, wrong password, nonexistent user, first login flag; test ChangePassword with valid old/new, wrong old, complexity violations
- [ ] T021 [P] [US1] Write unit tests for `LoginViewModel` in `tests/WpfTodoApp.Modules.Login.Tests/LoginViewModelTests.cs` — test LoginCommand with valid/wrong credentials, property change notification, error message display
- [ ] T022 [P] [US1] Write unit tests for `ChangePasswordViewModel` in `tests/WpfTodoApp.Modules.Login.Tests/ChangePasswordViewModelTests.cs` — test password match validation, complexity validation, successful change, error display

### Implementation for User Story 1

- [ ] T023 [US1] Create `src/WpfTodoApp.Services/Interfaces/IAuthService.cs` — interface with methods: `Authenticate(string username, string password) → AuthResult`, `ChangePassword(string username, string oldPassword, string newPassword) → ChangePasswordResult`; define `AuthResult` and `ChangePasswordResult` DTOs
- [ ] T024 [US1] Create `src/WpfTodoApp.Services/AuthService.cs` — implement IAuthService: EF Core query user, BCrypt.Verify password, BCrypt.HashPassword for new passwords, set IsFirstLogin=false after change, publish UserLoggedInEvent
- [ ] T025 [US1] Register `IAuthService` → `AuthService` in `App.xaml.cs` `RegisterTypes()` as singleton
- [ ] T026 [US1] Create `src/WpfTodoApp.Modules.Login/ViewModels/LoginViewModel.cs` — inherit BindableBase, properties: `Username`, `Password`, `ErrorMessage`, `IsLoading`; DelegateCommand `LoginCommand` calling `IAuthService.Authenticate()`; on success with IsFirstLogin=true navigate to ChangePasswordView, else publish UserLoggedInEvent
- [ ] T027 [US1] Create `src/WpfTodoApp.Modules.Login/ViewModels/ChangePasswordViewModel.cs` — inherit BindableBase, properties: `NewPassword`, `ConfirmPassword`, `ErrorMessage`; DelegateCommand `ChangePasswordCommand` with validation (length ≥6, alphanumeric, match confirm), call `IAuthService.ChangePassword()`
- [ ] T028 [US1] Create `src/WpfTodoApp.Modules.Login/Views/LoginView.xaml` — Panuon-styled login form: Username TextBox, Password PasswordBox, ErrorMessage TextBlock, Login Button bound to LoginCommand; zero code-behind beyond InitializeComponent()
- [ ] T029 [US1] Create `src/WpfTodoApp.Modules.Login/Views/ChangePasswordView.xaml` — Panuon-styled form: NewPassword PasswordBox, ConfirmPassword PasswordBox, ErrorMessage TextBlock, Confirm Button; password complexity hint label
- [ ] T030 [US1] Create `src/WpfTodoApp.Modules.Login/LoginModule.cs` — implement IModule, in `OnInitialized` register LoginView and ChangePasswordView for navigation via IRegionManager
- [ ] T031 [US1] Wire navigation: on app start, navigate to LoginView in ContentRegion; after login success, navigate to placeholder MainView (or direct to TodoList if ready)

**Checkpoint**: User Story 1 complete — login + password change fully functional and independently testable

---

## Phase 4: User Story 3 - 办事项核心管理 (Priority: P1) 🎯 MVP Core

**Goal**: 实现任务 CRUD，列表按创建时间降序排列，支持筛选（全部/已完成/未完成）

**Independent Test**: 登录 → 添加多个任务 → 验证排序 → 编辑任务 → 标记完成 → 切换筛选器 → 删除任务 → 验证删除

### Tests for User Story 3 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T032 [P] [US3] Write unit tests for `TodoService` in `tests/WpfTodoApp.Services.Tests/TodoServiceTests.cs` — test CreateTodo (valid/invalid title, default values), GetTodos (filter All/Completed/Incomplete, sort order), UpdateTodo, ToggleComplete, DeleteTodo; use InMemory SQLite database
- [ ] T033 [P] [US3] Write unit tests for `TodoListViewModel` in `tests/WpfTodoApp.Modules.TodoList.Tests/TodoListViewModelTests.cs` — test LoadTodos with different filters, AddTodoCommand, EditTodoCommand, ToggleCompleteCommand, DeleteTodoCommand, property changes, filter switching

### Implementation for User Story 3

- [ ] T034 [US3] Create `src/WpfTodoApp.Services/Interfaces/ITodoService.cs` — interface with methods: `GetTodos(int userId, TodoFilter filter)`, `CreateTodo(...)`, `UpdateTodo(...)`, `ToggleComplete(int todoId)`, `DeleteTodo(int todoId)`; define `TodoFilter` enum (All, Completed, Incomplete) and `TodoItemDto` record
- [ ] T035 [US3] Create `src/WpfTodoApp.Services/TodoService.cs` — implement ITodoService using EF Core; GetTodos sorts by CreatedAt descending, filters by IsCompleted per TodoFilter; CreateTodo sets CreatedAt=UtcNow, IsCompleted=false; all operations verify UserId ownership
- [ ] T036 [US3] Register `ITodoService` → `TodoService` in `App.xaml.cs` `RegisterTypes()` as scoped
- [ ] T037 [US3] Create `src/WpfTodoApp.Modules.TodoList/ViewModels/TodoListViewModel.cs` — inherit BindableBase, implement IDisposable; properties: `ObservableCollection<TodoItemDto> Todos`, `TodoFilter CurrentFilter`, `bool IsLoading`; DelegateCommands: `LoadTodosCommand`, `AddTodoCommand`, `EditTodoCommand`, `ToggleCompleteCommand`, `DeleteTodoCommand`, `FilterChangedCommand`; subscribe to UserLoggedInEvent to load user's todos
- [ ] T038 [US3] Create `src/WpfTodoApp.Modules.TodoList/Views/TodoListView.xaml` — Panuon-styled: filter ComboBox (全部/已完成/未完成任务), add Button; ListBox/ItemsControl with VirtualizingStackPanel for >100 items; each item shows title, created time, completion checkbox, edit/delete buttons
- [ ] T039 [US3] Create `src/WpfTodoApp.Modules.TodoList/Converters/BoolToStatusConverter.cs` — IValueConverter: true→"已完成" with strikethrough visual, false→"未完成"
- [ ] T040 [US3] Create `src/WpfTodoApp.Modules.TodoList/TodoListModule.cs` — implement IModule, register TodoListView for navigation in OnInitialized
- [ ] T041 [US3] Wire TodoList navigation: after login success, navigate to TodoListView in ContentRegion

**Checkpoint**: User Story 3 complete — Todo CRUD + filtering fully functional. MVP core delivered.

---

## Phase 5: User Story 4 - 任务优先级、截止日期与到期提醒 (Priority: P2)

**Goal**: 添加优先级和截止日期字段，实现到期前10分钟 Windows Toast 提醒

**Independent Test**: 创建带截止日期+5分钟的高优先级任务 → 验证列表显示优先级和截止日期 → 等待~5分钟 → 验证弹出提醒通知

**Prerequisite**: US3 (TodoList core) must be complete

### Tests for User Story 4 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T042 [P] [US4] Write unit tests for `ReminderService` in `tests/WpfTodoApp.Services.Tests/ReminderServiceTests.cs` — test CheckMissedReminders (returns overdue incomplete tasks), start/stop monitoring, timer-based reminder triggering (use testable timer abstraction), verify TodoDueReminderEvent published
- [ ] T043 [P] [US4] Write unit tests for `TodoEditViewModel` in `tests/WpfTodoApp.Modules.TodoList.Tests/TodoEditViewModelTests.cs` — test creating todo with due date, with priority, editing due date/priority, past due date warning, null due date (no reminder)

### Implementation for User Story 4

- [ ] T044 [US4] Create `src/WpfTodoApp.Services/Interfaces/IReminderService.cs` — interface: `StartMonitoring()`, `StopMonitoring()`, `CheckMissedReminders() → List<TodoItemDto>`, event `ReminderTriggered`
- [ ] T045 [US4] Create `src/WpfTodoApp.Services/ReminderService.cs` — implement IReminderService with System.Timers.Timer (60s interval); on each tick, query incomplete todos with DueDate within (now, now+10min) and not yet reminded; publish TodoDueReminderEvent; on StartMonitoring call CheckMissedReminders for overdue tasks; implement IDisposable
- [ ] T046 [US4] Register `IReminderService` → `ReminderService` in `App.xaml.cs` `RegisterTypes()` as singleton (start monitoring immediately after DI resolution)
- [ ] T047 [US4] Create `src/WpfTodoApp.Modules.TodoList/ViewModels/TodoEditViewModel.cs` — inherit BindableBase; properties: `Title`, `Description`, `DueDate` (DateTime?), `Priority` (TodoPriority), `IsEditing`, `ErrorMessage`; DelegateCommands: `SaveCommand` (validates title required, warns if due date past), `CancelCommand`; used for both create and edit modes
- [ ] T048 [US4] Create `src/WpfTodoApp.Modules.TodoList/Views/TodoEditView.xaml` — modal dialog with Title TextBox, Description TextBox, DueDate DatePicker, Priority ComboBox (高/中/低), Save/Cancel buttons; Panuon-styled
- [ ] T049 [US4] Create `src/WpfTodoApp.Modules.TodoList/Converters/PriorityToColorConverter.cs` — IValueConverter: High→Red, Medium→Orange, Low→Gray
- [ ] T050 [US4] Update TodoListViewModel to integrate TodoEditViewModel (open edit dialog, refresh list after save), add priority color display via PriorityToColorConverter
- [ ] T051 [US4] Update TodoListView to show DueDate column, Priority badge (colored), expired indicator; ensure VirtualizingStackPanel remains for performance
- [ ] T052 [US4] Create toast notification integration in `MainWindow.xaml.cs` — subscribe to TodoDueReminderEvent, create Windows Toast Notification with task title and due time, handle activation (navigate to TodoList)

**Checkpoint**: User Story 4 complete — priority, due date, and reminder system fully functional

---

## Phase 6: User Story 2 - 用户管理 (Priority: P2)

**Goal**: 管理员可查看、创建、编辑、删除用户；非管理员不可访问

**Independent Test**: 管理员登录 → 进入用户管理 → 创建新用户 → 验证列表 → 编辑密码 → 用新密码登录 → 管理员删除该用户 → 验证无法登录

**Note**: This phase is independent of US3/US4 and can run in parallel with them.

### Tests for User Story 2 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T053 [P] [US2] Write unit tests for `UserService` in `tests/WpfTodoApp.Services.Tests/UserServiceTests.cs` — test GetAllUsers (returns all users without password hash), CreateUser (valid input, duplicate username, password complexity), UpdateUserPassword (valid, complexity fail), DeleteUser (non-admin, cannot delete self, cannot delete admin), authorization checks
- [ ] T054 [P] [US2] Write unit tests for `UserManagementViewModel` in `tests/WpfTodoApp.Modules.UserManagement.Tests/UserManagementViewModelTests.cs` — test LoadUsers, AddUserCommand (success, duplicate, validation), EditPasswordCommand, DeleteUserCommand (success, admin protection, confirmation), non-admin access denied

### Implementation for User Story 2

- [ ] T055 [US2] Create `src/WpfTodoApp.Services/Interfaces/IUserService.cs` — interface: `GetAllUsers() → List<UserDto>`, `CreateUser(string username, string password) → CreateUserResult`, `UpdateUserPassword(int userId, string newPassword) → UpdatePasswordResult`, `DeleteUser(int userId) → DeleteUserResult`; define DTOs and result types
- [ ] T056 [US2] Create `src/WpfTodoApp.Services/UserService.cs` — implement IUserService: EF Core CRUD; CreateUser validates username uniqueness (case-insensitive), password complexity (length≥6, alphanumeric), bcrypt hash; DeleteUser prevents self-deletion and admin deletion; only admin can access
- [ ] T057 [US2] Register `IUserService` → `UserService` in `App.xaml.cs` `RegisterTypes()` as scoped
- [ ] T058 [US2] Create `src/WpfTodoApp.Modules.UserManagement/ViewModels/UserManagementViewModel.cs` — inherit BindableBase; properties: `ObservableCollection<UserDto> Users`, `string ErrorMessage`; DelegateCommands: `LoadUsersCommand`, `AddUserCommand`, `EditPasswordCommand`, `DeleteUserCommand`; implement IDisposable; check admin authorization on load
- [ ] T059 [US2] Create `src/WpfTodoApp.Modules.UserManagement/Views/UserManagementView.xaml` — Panuon-styled DataGrid or ListView showing Username, IsAdmin badge, actions (edit password, delete); Add User button opens inline form or modal dialog; delete requires confirmation dialog; non-admin users see "无权访问" message
- [ ] T060 [US2] Create `src/WpfTodoApp.Modules.UserManagement/UserManagementModule.cs` — implement IModule, register UserManagementView for navigation in OnInitialized
- [ ] T061 [US2] Wire UserManagement navigation: add navigation menu item (visible only to admin users per UserDto.IsAdmin); navigate to UserManagementView in ContentRegion

**Checkpoint**: User Story 2 complete — user management fully functional for admin users

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T062 Create navigation menu in `MainWindow.xaml` — MenuRegion with buttons for TodoList, User Management (admin-only visibility), bound to current user's IsAdmin property
- [ ] T063 [P] Implement error handling: global try-catch in App.xaml.cs for unhandled exceptions; user-friendly error messages in all ViewModels; log errors via ILogger
- [ ] T064 [P] Implement data persistence verification: ensure EF Core SaveChanges called after all mutations; handle DbUpdateException with user-friendly messages
- [ ] T065 [P] Style audit: verify all XAML files reference AppResources.xaml; no hardcoded colors/fonts; all custom styles use Panuon BasedOn
- [ ] T066 [P] Run code quality check: verify all methods ≤50 lines, files ≤800 lines, nesting ≤4 levels; refactor as needed
- [ ] T067 Run all unit tests: `dotnet test tests/` — verify all pass, coverage ≥80%
- [ ] T068 Run quickstart validation: execute all 5 validation scenarios from quickstart.md; fix any failures

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Setup — BLOCKS all user stories
- **US1 Login (Phase 3)**: Depends on Foundational — BLOCKS all user-facing features
- **US3 Todo Core (Phase 4)**: Depends on Foundational + US1 (login context) — can partially overlap
- **US4 Reminders (Phase 5)**: Depends on US3 (extends Todo) — strictly after US3
- **US2 User Mgmt (Phase 6)**: Depends on Foundational + US1 — can run in PARALLEL with US3+US4
- **Polish (Phase 7)**: Depends on all user stories complete

### User Story Dependencies

```text
Foundational (Phase 2)
    │
    ├── US1: Login (Phase 3) ── BLOCKS all ──┐
    │                                         │
    ├── US3: Todo Core (Phase 4) ─────────────┤
    │   └── US4: Reminders (Phase 5)           │
    │                                         │
    └── US2: User Mgmt (Phase 6) ──── parallel ┘
```

- **US1 (P1)**: After Foundational — no other story dependencies
- **US3 (P1)**: After Foundational — may integrate with US1 for user context, but independently testable
- **US4 (P2)**: After US3 — extends TodoItem with new fields and reminder service
- **US2 (P2)**: After Foundational — independent of US3/US4, can be developed in parallel

### Within Each User Story

- Tests MUST be written and FAIL before implementation (TDD)
- Service interface → Service implementation → ViewModel → View → Module registration
- Core implementation before integration
- Story complete before moving to next priority (except US2 which is parallel)

### Parallel Opportunities

- **Phase 1**: T002–T008 all marked [P] — can run in parallel (different csproj files)
- **Phase 2**: T011–T014 marked [P] — can run in parallel (different files)
- **US1 Tests**: T020–T022 marked [P] — can run in parallel
- **US3 Tests**: T032–T033 marked [P] — can run in parallel
- **US4 Tests**: T042–T043 marked [P] — can run in parallel
- **US2 Tests**: T053–T054 marked [P] — can run in parallel
- **Cross-phase**: US2 (Phase 6) can run in parallel with US3+US4 (Phases 4-5)
- **Phase 7**: T063–T066 marked [P] — can run in parallel

---

## Parallel Example: User Story 1

```bash
# Launch all tests together first:
Task: "Unit tests for AuthService in tests/WpfTodoApp.Services.Tests/AuthServiceTests.cs"
Task: "Unit tests for LoginViewModel in tests/WpfTodoApp.Modules.Login.Tests/LoginViewModelTests.cs"
Task: "Unit tests for ChangePasswordViewModel in tests/WpfTodoApp.Modules.Login.Tests/ChangePasswordViewModelTests.cs"

# After tests fail, launch independent implementation:
Task: "Create IAuthService interface in src/WpfTodoApp.Services/Interfaces/IAuthService.cs"
Task: "Create LoginViewModel in src/WpfTodoApp.Modules.Login/ViewModels/LoginViewModel.cs"  # depends on IAuthService
Task: "Create ChangePasswordViewModel in src/WpfTodoApp.Modules.Login/ViewModels/ChangePasswordViewModel.cs"  # depends on IAuthService
```

## Parallel Example: Phase 2 (Foundational)

```bash
# All models + events can be created simultaneously:
Task: "Create User.cs in src/WpfTodoApp.Core/Models/User.cs"
Task: "Create TodoItem.cs in src/WpfTodoApp.Core/Models/TodoItem.cs"
Task: "Create TodoPriority.cs in src/WpfTodoApp.Core/Models/TodoPriority.cs"
Task: "Create UserLoggedInEvent.cs in src/WpfTodoApp.Core/Events/UserLoggedInEvent.cs"
Task: "Create TodoDueReminderEvent.cs in src/WpfTodoApp.Core/Events/TodoDueReminderEvent.cs"
```

---

## Implementation Strategy

### MVP First (US1 + US3 Only)

1. Complete Phase 1: Setup (T001–T009)
2. Complete Phase 2: Foundational (T010–T019) — **CRITICAL BLOCKER**
3. Complete Phase 3: US1 Login (T020–T031) — **Entry point**
4. Complete Phase 4: US3 Todo Core (T032–T041) — **Core value**
5. **STOP and VALIDATE**: Test login + todo CRUD + filtering independently
6. Demo/deploy if ready — this is already a usable task management app!

### Incremental Delivery

1. Setup + Foundational → Foundation ready
2. + US1 Login → Users can authenticate (T020–T031)
3. + US3 Todo Core → Users can manage tasks (T032–T041) ← **MVP!**
4. + US4 Reminders → Priority + due date + notifications (T042–T052)
5. + US2 User Mgmt → Admin can manage users (T053–T061)
6. + Polish → Production-ready (T062–T068)

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Developer A: US1 Login (Phase 3)
3. Once US1 complete:
   - Developer A: US3 Todo Core + US4 Reminders (Phase 4→5)
   - Developer B: US2 User Management (Phase 6) — **parallel with Todo!**
4. All developers: Polish (Phase 7)

---

## Notes

- [P] tasks = different files, no dependencies — launch in parallel
- [Story] label maps task to specific user story for traceability
- Each user story should be independently completable and testable
- **Tests are NOT optional** — constitution Principle V mandates ≥80% coverage
- Write tests FIRST, verify they FAIL, then implement to GREEN
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
