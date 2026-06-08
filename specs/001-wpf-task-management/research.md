# Research: WPF 多模块任务管理系统

**Date**: 2026-06-08

## 1. Prism.Wpf 8.x 模块化架构

**Decision**: 采用 Prism.Wpf 的 `IModule` 接口 + `IModuleCatalog` 实现模块化加载。

**Rationale**:
- Prism 是 WPF MVVM 生态中最成熟的模块化框架，提供开箱即用的 `IModule`、`IRegionManager`、`IEventAggregator`
- 8.x 版本与 .NET 8 完全兼容，内置 DryIoc DI 容器，无需额外配置
- `IModuleCatalog` 支持代码注册和配置文件注册两种方式，本项目使用代码注册（`App.xaml.cs` 中 `ConfigureModuleCatalog`）

**Alternatives considered**:
- **CommunityToolkit.Mvvm**: 更轻量但无模块系统，需自行实现导航和模块加载
- **手动模块化（无框架）**: 工作量大，且宪法原则 I 明确要求 Prism
- **Prism.Unity**: 曾是 Prism 默认容器，但 8.x 已转向 DryIoc（更轻量、更快）

---

## 2. Panuon.WPF.UI 主题与控件

**Decision**: 使用 Panuon.WPF.UI 作为全局 UI 组件库，在 `AppResources.xaml` 中合并其主题资源。

**Rationale**:
- 提供完整的暗色/亮色主题系统，减少手写控件模板的需求
- 内置现代化控件（Window、Button、TextBox、DataGrid 等）开箱即用
- 支持 `BasedOn` 样式继承，符合宪法原则 III 的要求

**Key integration points**:
- `App.xaml` 启动时加载 Panuon 主题
- 所有自定义样式必须继承 Panuon 基础样式，使用 `BasedOn="{StaticResource {x:Type ControlType}}"`
- Panuon 的 Window 控件支持自定义标题栏，可用于主窗口 Shell

**Alternatives considered**:
- **Hand-built styles / MahApps.Metro**: Hand-built 违反原则 III（减少手写控件模板），MahApps.Metro 是另一个选择但 Panuon 中文文档更好
- **ModernWpf / WPF-UI (lepo.co)**: 也不错，但宪法已明确 Panuon.WPF.UI

---

## 3. 数据持久化 — SQLite + EF Core

**Decision**: 使用 SQLite 作为本地数据库，通过 `Microsoft.EntityFrameworkCore.Sqlite` 访问。

**Rationale**:
- 桌面应用无需服务端数据库，SQLite 是 .NET 生态中首选的嵌入式数据库
- EF Core 提供 Code-First 迁移、LINQ 查询、自动建表
- 数据库文件存储在 `%APPDATA%/WpfTodoApp/` 目录，用户级隔离

**Alternatives considered**:
- **LiteDB**: 无 SQL 的嵌入式 NoSQL，更简单但查询灵活性不如 EF Core
- **JSON 文件**: 简单场景可行，但多表关联、查询、并发写场景下复杂度急剧增加
- **SQL Server LocalDB**: 需要额外安装，对桌面应用过于沉重

---

## 4. 密码安全 — BCrypt

**Decision**: 使用 `BCrypt.Net-Next` 进行密码哈希存储。

**Rationale**:
- BCrypt 是行业标准的密码哈希算法，自动处理盐值和迭代次数
- 使用 `BCrypt.HashPassword(password)` 生成哈希，`BCrypt.Verify(password, hash)` 验证
- 原始密码永远不落盘，仅存储 bcrypt 哈希值

**Alternatives considered**:
- **PBKDF2 (ASP.NET Identity)**: 也不错，但引入 Identity 框架对桌面应用过重
- **SHA256 + 盐值**: 不够安全，SHA 系列不抗暴力破解
- **Argon2**: 最新标准，但 .NET 生态中库不如 BCrypt 成熟

---

## 5. Windows Toast Notification（到期提醒）

**Decision**: 使用 `Microsoft.Toolkit.Uwp.Notifications` 发送 Windows 10/11 Toast 通知。

**Rationale**:
- WPF 应用可通过此库发送原生 Windows Toast Notification
- 支持按钮交互（如"知道了"、"查看任务"），用户体验好
- 无需 UWP 打包，纯桌面应用即可使用

**Implementation notes**:
- 需要在启动时注册 AUMID（Application User Model ID）以启用 Toast
- 提醒服务 (`IReminderService`) 在后台使用 `System.Timers.Timer` 每分钟轮询检查到期任务
- 应用关闭期间到期的任务，在下次启动时由 ReminderService 补发通知

**Alternatives considered**:
- **WPF 内嵌弹窗**: 应用失焦时用户看不到
- **Task Scheduler**: 创建 Windows 计划任务过于重量，且不便于卸载时清理

---

## 6. 测试策略

**Decision**: 采用 xUnit + Moq，ViewModels 和 Services 全覆盖。

**Rationale**:
- xUnit 是 .NET 生态最广泛使用的测试框架
- Moq 提供强类型 mocking，适合模拟 Prism 服务（`IRegionManager`、`IEventAggregator` 等）
- ViewModels 通过 Mock 依赖注入进行测试，Services 通过 InMemory SQLite 进行测试

**Coverage targets**:
- Services: ≥80%（核心业务逻辑）
- ViewModels: ≥80%（UI 逻辑）
- Models: 自动属性无需单独测试（覆盖率不计入）

---

## 7. 项目结构决策

**Decision**: 采用多项目结构（7 个 src 项目 + 对应 test 项目），而非单项目。

**Rationale**:
- 宪法原则 I 要求模块化（`IModule`），每个模块应为独立项目以强制依赖方向正确
- 原则 IV 要求文件 ≤800 行 — 独立项目自然限制文件大小
- 分离 Core / Services / Modules 三层防止模块间耦合

**Dependency direction**: Modules → Services (interfaces) ← Core (Models) ← App (composition root)
