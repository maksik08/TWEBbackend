namespace ProjectBackend.api.Models.Query
{
    public sealed class NotificationQueryOptions : PagedQueryOptions
    {
        public bool UnreadOnly { get; init; }
    }
}
