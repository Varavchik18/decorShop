public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    Task<int> CompleteAsync();
}