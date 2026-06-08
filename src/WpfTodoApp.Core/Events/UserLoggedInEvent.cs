using Prism.Events;

namespace WpfTodoApp.Core.Events;

public class UserLoggedInEvent : PubSubEvent<UserLoggedInEvent.Payload>
{
    public record Payload(int UserId, string Username, bool IsAdmin);
}
