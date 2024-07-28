using DecorStore.BL.Models;

public interface ICategoryRepository
{
    Task<CategoryAggregate> GetAggregateBySectionIdAsync(int sectionId);
    Task<Section> GetSectionBySectionIdAsync(int sectionId);

    Task<List<Section>> GetAllCategoriesAsync();
    Task AddAsync(CategoryAggregate aggregate);
    Task UpdateAsync(CategoryAggregate aggregate);
    Task DeleteAsync(int sectionId);
    void RemoveAggregate(CategoryAggregate aggregate);
    Task<Category> GetCategoryByIdAsync(int categoryId);
    Task<Subcategory> GetSubCategoryByIdAsync(int subCategoryId);


    // Checks for validation
    Task<bool> IsSectionNameUniqueAsync(string name);
    Task<bool> IsCategoryNameUniqueInSectionAsync(string name, int sectionId);
    Task<bool> IsSubCategoryNameUniqueInCategoryAsync(string name, int categoryId);

}
