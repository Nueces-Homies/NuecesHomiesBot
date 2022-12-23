using System.Collections.Immutable;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Books.v1;
using Google.Apis.Books.v1.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace Integrations.GoogleApi
{
    public interface IGoogle
    {
        Task<EventsResponse> GetEventsAsync(string calendarId);
        Task<Book?> LookupBookAsync(string isbn);
    }

    public class Google : IGoogle
    {
        private readonly CalendarService calendarService;
        private readonly BooksService booksService;
    
        public Google(string credentialsJson)
        {
            var credentials = GoogleCredential
                .FromJson(credentialsJson)
                .CreateScoped(CalendarService.Scope.CalendarEventsReadonly, BooksService.Scope.Books);

            var initializer = new BaseClientService.Initializer() { HttpClientInitializer = credentials };

            this.calendarService = new CalendarService(initializer);
            this.booksService = new BooksService(initializer);
        }

        public async Task<EventsResponse> GetEventsAsync(string calendarId)
        {
            EventsResource.ListRequest request = this.calendarService.Events.List(calendarId);
            request.SingleEvents = true;
            request.ShowDeleted = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.TimeMin = DateTime.UtcNow.AddHours(-3);
            request.TimeMax = DateTime.UtcNow.AddYears(3);

            var result = await request.ExecuteAsync();

            if (result == null)
            {
                throw new Exception("Got null result");
            }

            var events = result.Items.Select(e => new CalendarEvent(
                Title: e.Summary,
                Description: e.Description,
                StartTime: e.Start.DateTime,
                EndTime: e.End.DateTime,
                Deleted: e.Status == "cancelled")).ToImmutableArray();

            if (!result.Updated.HasValue)
            {
                throw new Exception("Updated does not have a value");
            }
        
            return new EventsResponse(events, result.Updated.Value);
        }

        public async Task<Book?> LookupBookAsync(string isbn)
        {
            // TODO: Validate ISBN?
            string cleanedIsbn = isbn.Replace("-", "");
            string query = $"isbn:{cleanedIsbn}";

            var request = this.booksService.Volumes.List(query);
            var response = await request.ExecuteAsync();

            if (response == null)
            {
                throw new Exception("Unable to get books");
            }

            if (response.TotalItems.GetValueOrDefault() == 0)
            {
                return null;
            }

            var book = response.Items.First().VolumeInfo;
            return new Book(book.Title, book.Description, book.Authors,isbn, book.PageCount.GetValueOrDefault());
        }
    }
}