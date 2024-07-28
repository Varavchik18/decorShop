using DecorStore.BL.Models;

public interface ICategoryRepository
{
    Task<CategoryAggregate> GetBySectionIdAsync(int sectionId);
    Task<List<Section>> GetAllCategoriesAsync();
    Task AddAsync(CategoryAggregate aggregate);
    Task UpdateAsync(CategoryAggregate aggregate);
    Task DeleteAsync(int sectionId);

    // Checks for validation
    Task<bool> IsSectionNameUniqueAsync(string name);
    Task<bool> IsCategoryNameUniqueInSectionAsync(string name, int sectionId);
}
