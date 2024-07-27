using DecorStore.Data.Shared;

namespace DecorStore.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private CategoryRepository _categoryRepository;
        public UnitOfWork(AppDbContext context)
        {

            _context = context;
        }

        public ICategoryRepository Categories
        {
            get
            {
                return _categoryRepository ??= new CategoryRepository(_context);
            }
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
