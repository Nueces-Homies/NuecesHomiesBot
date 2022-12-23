namespace Integrations.GoogleApi;
public record CalendarEvent(string Title, string Description, DateTime? StartTime, DateTime? EndTime, bool Deleted);

public record struct EventsResponse(IList<CalendarEvent> Events, DateTime Updated);

public record Book(string Title, string Description, IList<string> Author, string ISBN, int PageCount);