namespace ProjectBackend.api.Data
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
