# Implementation Plan: WPF 多模块任务管理系统

**Branch**: `001-wpf-task-management` | **Date**: 2026-06-08 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `/specs/001-wpf-task-management/spec.md`

## Summary

构建一个基于 Prism.Wpf + Panuon.WPF.UI 的多模块 WPF 桌面应用，包含三个核心模块：
1. **登录模块**：用户认证、默认 admin 账户管理、首次登录强制密码修改
2. **用户管理模块**：管理员对系统用户的 CRUD 操作
3. **TodoList 模块**：个人任务管理，支持排序、筛选、优先级、截止日期和到期提醒

技术方案采用 Prism 模块化架构，各功能模块作为独立 `IModule` 加载，通过
`IRegionManager` 实现导航，`IEventAggregator` 实现跨模块通信。

## Technical Context

**Language/Version**: C# 12 / .NET 8

**Primary Dependencies**:
- Prism.Wpf (8.x) — MVVM 框架 + DryIoc DI 容器
- Panuon.WPF.UI — 现代化 WPF 控件库与主题系统
- Microsoft.Extensions.Logging — 结构化日志
- xUnit + Moq — 单元测试框架

**Storage**: SQLite（本地数据库），通过 EF Core SQLite provider 访问

**Testing**: xUnit + Moq；ViewModels 和 Services 层覆盖目标 ≥80%

**Target Platform**: Windows 10/11 桌面应用（.NET 8 WPF）

**Project Type**: WPF 桌面应用（多模块 Prism 架构）

**Performance Goals**: 500 条任务列表流畅滚动（60fps），内存占用 <200MB

**Constraints**: 单机运行、本地数据存储、离线可用；提醒使用 Windows Toast Notification

**Scale/Scope**: 单用户桌面应用（多用户账号，但同一时间仅一个用户会话）；3 个功能模块

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Principle | Requirement | Status |
|-----------|------------|--------|
| **I. Prism MVVM** | Views 零 code-behind；ViewModels 继承 `BindableBase`，使用 `SetProperty<T>` 和 `DelegateCommand`；导航通过 `IRegionManager`；跨 VM 通信通过 `IEventAggregator`；模块封装为 `IModule` | ✅ 设计符合 |
| **II. Interface-Oriented Services + DI** | 所有服务有对应接口；通过 `IContainerRegistry` 注册；构造函数注入；禁止 Service Locator | ✅ 设计符合 — `IAuthService`、`IUserService`、`ITodoService` 等 |
| **III. Panuon.WPF.UI** | 应用引用 Panuon 主题；自定义样式基于 Panuon `BasedOn`；ResourceDictionary 集中管理 | ✅ 设计符合 — 全局 `AppResources.xaml` 合并 Panuon 主题 |
| **IV. Code Quality** | 方法 ≤50 行、文件 ≤800 行、嵌套 ≤4 层、不可变性优先、DRY | ⏳ 实现阶段逐文件检查 |
| **V. Performance & Testing** | ViewModels 实现 `IDisposable`；`VirtualizingStackPanel` 用于 >100 项列表；≥80% 测试覆盖率；AAA 模式 | ✅ 设计符合 — TodoList 使用虚拟化；全部 ViewModel/Service 有对应测试 |

**Gate Result**: ✅ PASS — 所有原则在设计中均有对应约束，无违规需要豁免。Phase 1 设计完成后复查。

## Project Structure

### Documentation (this feature)

```text
specs/001-wpf-task-management/
├── plan.md              # This file
├── research.md          # Phase 0 output
├── data-model.md        # Phase 1 output
├── quickstart.md        # Phase 1 output
├── contracts/           # Phase 1 output (模块接口契约)
└── tasks.md             # Phase 2 output (/speckit-tasks)
```

### Source Code (repository root)

> **Constitution v1.1.2**: 解决方案中只有 `WpfTodoApp` 一个 WPF Application 启动项目，
> 其他所有模块项目均为 WPF Class Library。详见 `.specify/memory/constitution.md`。

```text
WpfTodoApp.sln                              # 单一解决方案
│
├── src/WpfTodoApp/                         # [WPF Application] 唯一启动项目
│   ├── App.xaml                            # PrismApplication 入口
│   ├── App.xaml.cs                         # DI 组合根 + ModuleCatalog
│   ├── MainWindow.xaml                     # Shell + Prism Regions
│   ├── MainWindow.xaml.cs
│   ├── Resources/
│   │   └── AppResources.xaml               # Panuon 主题合并 + 自定义样式
│   └── WpfTodoApp.csproj                   # UseWPF=true (Application)
│
├── src/WpfTodoApp.Core/                    # [.NET Class Library] 无 WPF 依赖
│   ├── Models/
│   │   ├── User.cs
│   │   └── TodoItem.cs
│   ├── Events/
│   │   ├── UserLoggedInEvent.cs
│   │   └── TodoDueReminderEvent.cs
│   └── WpfTodoApp.Core.csproj              # 纯类库 (无 UseWPF)
│
├── src/WpfTodoApp.Services/                # [.NET Class Library] 无 WPF 依赖
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
│   └── WpfTodoApp.Services.csproj          # 纯类库 (无 UseWPF)
│
├── src/WpfTodoApp.Modules.Login/           # [WPF Class Library] 登录模块
│   ├── Views/
│   │   ├── LoginView.xaml
│   │   └── ChangePasswordView.xaml
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   └── ChangePasswordViewModel.cs
│   ├── LoginModule.cs                      # IModule 实现
│   └── WpfTodoApp.Modules.Login.csproj     # UseWPF=true (Class Library)
│
├── src/WpfTodoApp.Modules.UserManagement/  # [WPF Class Library] 用户管理模块
│   ├── Views/
│   │   └── UserManagementView.xaml
│   ├── ViewModels/
│   │   └── UserManagementViewModel.cs
│   ├── UserManagementModule.cs
│   └── WpfTodoApp.Modules.UserManagement.csproj  # UseWPF=true (Class Library)
│
├── src/WpfTodoApp.Modules.TodoList/        # [WPF Class Library] TodoList 模块
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
│   └── WpfTodoApp.Modules.TodoList.csproj  # UseWPF=true (Class Library)
│
tests/
├── WpfTodoApp.Core.Tests/                  # [xUnit] 模型测试
├── WpfTodoApp.Services.Tests/              # [xUnit] 服务层测试
├── WpfTodoApp.Modules.Login.Tests/         # [xUnit] 登录模块测试
├── WpfTodoApp.Modules.UserManagement.Tests/ # [xUnit] 用户管理测试
├── WpfTodoApp.Modules.TodoList.Tests/      # [xUnit] TodoList 测试
└── WpfTodoApp.Integration.Tests/           # [xUnit] 集成测试(可选)
```

**Structure Decision**: 采用 Prism 多模块架构，遵循宪法 v1.1.2 单启动项目规则：

- **`WpfTodoApp`** 是解决方案中**唯一的 WPF Application 项目**（启动/Shell），其他所有
  模块项目（Login、UserManagement、TodoList）均为 **WPF Class Library** 项目，不可独立运行。
- 每个功能模块实现 `IModule` 接口，由启动项目通过 `IModuleCatalog` 加载。
- 共享的 Models、Events 放在 Core 库（纯 .NET Class Library，无 WPF 依赖）。
- 服务接口和实现在 Services 库（纯 .NET Class Library，无 WPF 依赖），所有模块依赖
  接口层而非实现层。
- 该结构遵循宪法 Principle I（Prism 模块化）、Principle IV（文件组织）、以及
  Technical Standards 的 Project Type Requirements（单启动 + WPF 类库模块）。

## Complexity Tracking

> 无宪法违规，无需豁免记录。

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| — | — | — |
