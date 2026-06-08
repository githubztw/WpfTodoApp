using Prism.Events;

namespace WpfTodoApp.Core.Events;

public class TodoDueReminderEvent : PubSubEvent<TodoDueReminderEvent.Payload>
{
    public record Payload(int TodoId, string Title, DateTime DueDate);
}
