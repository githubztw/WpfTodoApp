# User Management API Contract

**Module**: WpfTodoApp.Modules.UserManagement
**Service Interface**: `IUserService`

## GetAllUsers

```text
GetAllUsers() → List<UserDto>
```

- **Authorization**: 仅管理员可调用
- **Output**: 系统中所有用户的列表（不含密码哈希），每个 `UserDto` 包含 `Id`, `Username`, `IsAdmin`, `IsFirstLogin`

## CreateUser

```text
CreateUser(username: string, password: string) → CreateUserResult
```

- **Authorization**: 仅管理员可调用
- **Input**: 用户名、密码
- **Validation**:
  - 用户名长度 3–50，只能包含字母、数字、下划线
  - 用户名必须唯一
  - 密码满足复杂度（≥6位 + 含字母和数字）
- **Output**: `CreateUserResult` 包含 `Success (bool)`, `UserId (int?)`, `ErrorMessage (string?)`
- **Behavior**: 创建用户 → 密码哈希 → 保存到数据库

## UpdateUserPassword

```text
UpdateUserPassword(userId: int, newPassword: string) → UpdatePasswordResult
```

- **Authorization**: 仅管理员可调用
- **Input**: 目标用户 ID、新密码
- **Validation**: 新密码满足复杂度要求
- **Output**: `UpdatePasswordResult` 包含 `Success (bool)`, `ErrorMessage (string?)`
- **Constraint**: 不能修改 `admin` 账户以外的管理员密码（如存在其他管理员）

## DeleteUser

```text
DeleteUser(userId: int) → DeleteUserResult
```

- **Authorization**: 仅管理员可调用
- **Input**: 目标用户 ID
- **Output**: `DeleteUserResult` 包含 `Success (bool)`, `ErrorMessage (string?)`
- **Constraints**:
  - 不能删除自身（管理员不能删除自己）
  - 不能删除 admin 账户
  - 删除操作级联删除该用户的所有 TodoItems（ON DELETE CASCADE）
