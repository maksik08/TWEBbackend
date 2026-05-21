namespace ProjectBackend.DataAccess
{
    public interface IDatabaseInitializer
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
