namespace ProjectBackend.Domain.Entities
{
    public enum ServiceRequestStatus
    {
        Submitted = 0,
        Accepted = 1,
        Assigned = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5
    }
}
