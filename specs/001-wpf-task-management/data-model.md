# Data Model: WPF 多模块任务管理系统

**Date**: 2026-06-08

## Entity Relationship

```text
┌──────────┐       ┌──────────────┐
│   User   │ 1───* │  TodoItem    │
├──────────┤       ├──────────────┤
│ Id (PK)  │       │ Id (PK)      │
│ Username │       │ Title        │
│ Password │       │ Description  │
│ IsAdmin  │       │ CreatedAt    │
│ IsFirst  │       │ DueDate?     │
│ Login    │       │ Priority     │
└──────────┘       │ IsCompleted  │
                   │ UserId (FK)  │
                   └──────────────┘
```

## Entity Definitions

### User（用户）

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `Id` | `int` | PK, auto-increment | 用户唯一标识 |
| `Username` | `string` | Required, max 50, unique index | 登录用户名 |
| `PasswordHash` | `string` | Required, max 200 | bcrypt 密码哈希 |
| `IsAdmin` | `bool` | Required, default false | 是否为管理员 |
| `IsFirstLogin` | `bool` | Required, default false | 是否首次登录（需强制修改密码） |

**Validation Rules**:
- `Username`: 长度 3–50 字符，只能包含字母、数字、下划线，必须唯一
- `PasswordHash`: 非空，由 `BCrypt.HashPassword()` 生成
- `IsAdmin`: admin 账户必须为 true，且不可被删除或降级
- `IsFirstLogin`: 新创建的普通用户为 false，默认 admin 初始为 true

**State Transitions**:
```text
首次登录 [IsFirstLogin=true] ──(修改密码成功)──▶ 正常状态 [IsFirstLogin=false]
```

---

### TodoItem（办事项）

| Field | Type | Constraints | Description |
|-------|------|-------------|-------------|
| `Id` | `int` | PK, auto-increment | 任务唯一标识 |
| `Title` | `string` | Required, max 200 | 任务标题 |
| `Description` | `string` | Optional, max 1000 | 任务详细描述 |
| `CreatedAt` | `DateTime` | Required, UTC | 创建时间（用于排序） |
| `DueDate` | `DateTime?` | Optional | 截止日期 |
| `Priority` | `TodoPriority` | Required, default Medium | 优先级枚举 |
| `IsCompleted` | `bool` | Required, default false | 完成状态 |
| `UserId` | `int` | Required, FK → User.Id | 所属用户 |

**Validation Rules**:
- `Title`: 必填，长度 1–200 字符
- `DueDate`: 可选，若设置且已过去，标记为"已过期"但允许保存
- `Priority`: 枚举值 High (高), Medium (中), Low (低)

**Enums**:
```csharp
public enum TodoPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}
```

**State Transitions**:
```text
未完成 [IsCompleted=false] ──(标记完成)──▶ 已完成 [IsCompleted=true]
已完成 [IsCompleted=true] ──(取消完成)──▶ 未完成 [IsCompleted=false]
任意状态 ──(删除)──▶ 移除
```

## Database Schema (SQLite)

```sql
CREATE TABLE Users (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Username    TEXT NOT NULL UNIQUE COLLATE NOCASE,
    PasswordHash TEXT NOT NULL,
    IsAdmin     INTEGER NOT NULL DEFAULT 0,
    IsFirstLogin INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE TodoItems (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Title       TEXT NOT NULL,
    Description TEXT,
    CreatedAt   TEXT NOT NULL,       -- ISO 8601 UTC
    DueDate     TEXT,                -- ISO 8601, nullable
    Priority    INTEGER NOT NULL DEFAULT 1,  -- 0=Low, 1=Medium, 2=High
    IsCompleted INTEGER NOT NULL DEFAULT 0,
    UserId      INTEGER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);

CREATE INDEX IX_TodoItems_UserId ON TodoItems(UserId);
CREATE INDEX IX_TodoItems_UserId_IsCompleted ON TodoItems(UserId, IsCompleted);
```

## Seed Data

应用首次启动时自动创建：

```csharp
new User
{
    Username = "admin",
    PasswordHash = BCrypt.HashPassword("123456"),
    IsAdmin = true,
    IsFirstLogin = true
}
```
