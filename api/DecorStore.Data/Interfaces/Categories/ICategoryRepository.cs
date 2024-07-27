public interface ICategoryRepository
{
    Task<CategoryAggregate> GetBySectionIdAsync(int sectionId);
    Task<List<Section>> GetAllCategoriesAsync();
    Task AddAsync(CategoryAggregate aggregate);
    Task UpdateAsync(CategoryAggregate aggregate);
    Task DeleteAsync(int sectionId);
    Task<bool> IsSectionNameUniqueAsync(string name);
}
