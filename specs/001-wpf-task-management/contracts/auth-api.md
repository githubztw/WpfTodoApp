# Auth API Contract

**Module**: WpfTodoApp.Modules.Login
**Service Interface**: `IAuthService`

## Authenticate

```text
Authenticate(username: string, password: string) → AuthResult
```

- **Input**: 用户名（明文）、密码（明文）
- **Output**: `AuthResult` 包含 `Success (bool)`, `IsFirstLogin (bool)`, `ErrorMessage (string?)`
- **Behavior**:
  - 从数据库查找用户 → 如用户不存在，返回 `{Success=false, ErrorMessage="用户名或密码错误"}`
  - 验证密码 → 如密码不匹配，返回 `{Success=false, ErrorMessage="用户名或密码错误"}`
  - 如匹配且 `IsFirstLogin=true`，返回 `{Success=true, IsFirstLogin=true}`
  - 如匹配且 `IsFirstLogin=false`，返回 `{Success=true, IsFirstLogin=false}`

## ChangePassword

```text
ChangePassword(username: string, oldPassword: string, newPassword: string) → ChangePasswordResult
```

- **Input**: 用户名、旧密码、新密码
- **Output**: `ChangePasswordResult` 包含 `Success (bool)`, `ErrorMessage (string?)`
- **Validation**:
  - 新密码长度 ≥6 位
  - 新密码必须包含字母和数字
  - 新密码与旧密码不能相同
  - 两次输入的新密码必须一致（UI 层校验）
- **Behavior**:
  - 验证旧密码 → 不匹配则拒绝
  - 验证新密码复杂度 → 不满足则返回具体错误信息
  - 更新 `PasswordHash` + 设置 `IsFirstLogin=false`
  - 发布 `UserLoggedInEvent`

## Events

| Event | Payload | Publisher | Subscribers |
|-------|---------|-----------|-------------|
| `UserLoggedInEvent` | `UserId (int)`, `Username (string)` | `AuthService` (after successful login or password change) | MainWindow (navigate), TodoListModule (load user data) |
