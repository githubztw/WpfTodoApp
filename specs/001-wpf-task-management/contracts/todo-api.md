# Todo API Contract

**Module**: WpfTodoApp.Modules.TodoList
**Service Interface**: `ITodoService`

## GetTodos

```text
GetTodos(userId: int, filter: TodoFilter) → List<TodoItemDto>
```

- **Input**: 用户 ID、筛选条件
- **Filter options**: `All` | `Completed` | `Incomplete`
- **Output**: 任务列表，按 `CreatedAt` 降序排列，每个 `TodoItemDto` 包含全部字段

## CreateTodo

```text
CreateTodo(userId: int, title: string, description: string?, dueDate: DateTime?, priority: TodoPriority) → TodoItemDto
```

- **Input**: 用户 ID、标题（必填）、描述（可选）、截止日期（可选）、优先级
- **Validation**: 标题必填，1–200 字符
- **Behavior**: 创建任务 → `CreatedAt = DateTime.UtcNow` → `IsCompleted = false` → 返回完整 DTO

## UpdateTodo

```text
UpdateTodo(todoId: int, title: string, description: string?, dueDate: DateTime?, priority: TodoPriority) → TodoItemDto
```

- **Input**: 任务 ID、更新后的字段值
- **Validation**: 标题必填，1–200 字符；验证任务属于当前用户
- **Behavior**: 更新所有字段 → 返回完整 DTO

## ToggleComplete

```text
ToggleComplete(todoId: int) → TodoItemDto
```

- **Input**: 任务 ID
- **Behavior**: 切换 `IsCompleted` 状态 → 返回更新后的 DTO

## DeleteTodo

```text
DeleteTodo(todoId: int) → bool
```

- **Input**: 任务 ID
- **Output**: `true` 删除成功，`false` 任务不存在
- **Behavior**: 删除前需要用户确认（UI 层）

---

## Reminder API Contract

**Service Interface**: `IReminderService`

## StartMonitoring

```text
StartMonitoring() → void
```

- **Behavior**: 启动后台定时器（每分钟轮询），检查所有未完成且有截止日期的任务
- 当系统时间 ≥ `DueDate - 10 minutes` 时，触发提醒

## StopMonitoring

```text
StopMonitoring() → void
```

- **Behavior**: 停止后台定时器，释放资源

## CheckMissedReminders

```text
CheckMissedReminders() → List<TodoItemDto>
```

- **Behavior**: 应用启动时调用，查找所有已过期且未提醒的任务
- **Output**: 需要补发提醒的任务列表

## Events

| Event | Payload | Publisher | Subscribers |
|-------|---------|-----------|-------------|
| `TodoDueReminderEvent` | `TodoId (int)`, `Title (string)`, `DueDate (DateTime)` | `IReminderService` | MainWindow / Shell (show toast) |
