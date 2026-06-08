# Quickstart: WPF 多模块任务管理系统

**Date**: 2026-06-08

## Prerequisites

- Windows 10/11 (x64)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (推荐) 或 `dotnet` CLI

## Setup

```bash
# 1. 克隆仓库（或本地已有）
cd spec-kit_test

# 2. 还原 NuGet 包
dotnet restore src/WpfTodoApp/WpfTodoApp.csproj

# 3. 生成 EF Core 迁移（首次）
dotnet ef database update --project src/WpfTodoApp.Services --startup-project src/WpfTodoApp

# 4. 构建
dotnet build src/WpfTodoApp/WpfTodoApp.csproj --configuration Debug
```

## Run

```bash
# CLI 启动
dotnet run --project src/WpfTodoApp/WpfTodoApp.csproj

# 或直接在 Visual Studio 中按 F5 启动
```

## Validation Scenarios

### VS-1: 首次登录 + 密码修改

1. 启动应用 → 显示登录界面
2. 输入 `admin` / `123456` → 点击登录
3. **验证**: 界面跳转到密码修改页，提示"首次登录请修改密码"
4. 输入新密码 `Admin123` → 确认密码 `Admin123` → 点击保存
5. **验证**: 跳转至主界面
6. 关闭应用，重新启动
7. 输入 `admin` / `123456` → 点击登录
8. **验证**: 显示"用户名或密码错误"，旧密码已失效
9. 输入 `admin` / `Admin123` → 点击登录
10. **验证**: 成功进入主界面

### VS-2: 管理员创建普通用户

1. 以 `admin` 登录
2. 导航至"用户管理"模块
3. 点击"添加用户" → 输入用户名 `testuser`、密码 `Test123`
4. **验证**: 用户出现在列表中
5. 关闭应用，以 `testuser` / `Test123` 登录
6. **验证**: 直接进入主界面（无需修改密码），且用户管理入口不可见

### VS-3: Todo CRUD + 筛选

1. 以任意用户登录
2. 导航至 TodoList 模块
3. 点击"添加任务" → 输入标题"买菜" → 保存
4. 再添加"写报告" → 保存
5. 再添加"健身" → 保存
6. **验证**: 列表显示 3 个任务，按创建时间降序（"健身"在最前）
7. 点击"健身"标记为完成
8. 筛选器切换至"已完成任务"
9. **验证**: 仅显示"健身"
10. 切换至"未完成任务"
11. **验证**: 显示"写报告"和"买菜"
12. 切换至"全部任务"
13. **验证**: 显示全部 3 个任务

### VS-4: 截止日期提醒

1. 添加任务"测试提醒" → 设置截止日期为当前时间 +2 分钟 → 优先级"高"
2. 等待约 2 分钟
3. **验证**: 在截止前 2 分钟（截止时间 -10 分钟，即任务创建后约 0 分钟内...）

   *修正*: 设置截止日期为当前时间 +15 分钟 → 等待 5-6 分钟
4. **验证**: 弹出 Windows Toast 通知，显示"任务即将到期: 测试提醒"

### VS-5: 启动补发提醒

1. 创建任务，设置截止日期为过去的时间 → 保存
2. 关闭应用
3. 重新启动应用
4. **验证**: 启动后立即弹出提醒通知（补发已过期的提醒）

## Run Tests

```bash
# 运行所有单元测试
dotnet test tests/

# 运行特定模块测试
dotnet test tests/WpfTodoApp.Services.Tests/
dotnet test tests/WpfTodoApp.Modules.Login.Tests/

# 带覆盖率报告
dotnet test tests/ --collect:"XPlat Code Coverage"
```

## Expected Test Results

- 所有单元测试通过（绿灯）
- 代码覆盖率 ≥80%（Services + ViewModels 层）
- 无跳过（Skipped）测试
