namespace ProjectBackend.Domain.Query
{
    public sealed class NotificationQueryOptions : PagedQueryOptions
    {
        public bool UnreadOnly { get; init; }
    }
}
