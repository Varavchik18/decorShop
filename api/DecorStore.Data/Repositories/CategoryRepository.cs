
using DecorStore.BL.Models;
using DecorStore.Data.Shared;
using Microsoft.EntityFrameworkCore;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryAggregate> GetAggregateBySectionIdAsync(int sectionId)
    {
        var section = await GetSectionBySectionIdAsync(sectionId);

        if (section == null)
            return null;

        var aggregate = new CategoryAggregate(section);
        foreach (var category in section.Categories)
        {
            aggregate.AddCategory(category);
            foreach (var subcategory in category.Subcategories)
            {
                aggregate.AddSubcategory(subcategory);
            }
        }

        return aggregate;
    }

    public async Task<Section> GetSectionBySectionIdAsync(int sectionId)
    {
        var section = await _context.Sections
            .Include(s => s.Categories)
            .ThenInclude(c => c.Subcategories)
            .FirstOrDefaultAsync(s => s.Id == sectionId);


        if (section is null)
            return null;

        return section;
    }


    public async Task AddAsync(CategoryAggregate aggregate)
    {
        await _context.Sections.AddAsync(aggregate.Section);
        await _context.Categories.AddRangeAsync(aggregate.Categories);
        await _context.Subcategories.AddRangeAsync(aggregate.Subcategories);
    }

    public async Task UpdateAsync(CategoryAggregate aggregate)
    {
        _context.Sections.Update(aggregate.Section);
        _context.Categories.UpdateRange(aggregate.Categories);
        _context.Subcategories.UpdateRange(aggregate.Subcategories);
    }

    public async Task DeleteAsync(int sectionId)
    {
        var section = await _context.Sections.FindAsync(sectionId);
        if (section != null)
        {
            _context.Sections.Remove(section);
        }
    }

    public void RemoveAggregate(CategoryAggregate aggregate)
    {
        _context.Subcategories.RemoveRange(aggregate.Subcategories);
        _context.Categories.RemoveRange(aggregate.Categories);
        _context.Sections.Remove(aggregate.Section);
    }


    public async Task<List<Section>> GetAllCategoriesAsync()
    {
        var sectionList = await _context.Sections
            .Include(s => s.Categories)
            .ThenInclude(c => c.Subcategories)
            .ToListAsync();

        if (sectionList == null || sectionList.Count == 0)
            return null;

        return sectionList;
    }

    public async Task<Category> GetCategoryByIdAsync(int categoryId)
    {
        return await _context.Categories
            .Include(c => c.Subcategories)
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<Subcategory> GetSubCategoryByIdAsync(int subCategoryId)
    {
        return await _context.Subcategories
            .FirstOrDefaultAsync(c => c.Id == subCategoryId);
    }
    public async Task<bool> IsCategoryNameUniqueInSectionAsync(string name, int sectionId) => !await _context.Categories.AnyAsync(c => c.Name == name && c.SectionId == sectionId);

    public async Task<bool> IsSectionNameUniqueAsync(string name) => !await _context.Sections.AnyAsync(s => s.Name == name);
    public async Task<bool> IsSubCategoryNameUniqueInCategoryAsync(string name, int categoryId) => !await _context.Subcategories.AnyAsync(sc => sc.Name == name && sc.CategoryId == categoryId);
}