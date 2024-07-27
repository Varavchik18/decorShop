﻿
using DecorStore.Data.Shared;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryAggregate> GetBySectionIdAsync(int sectionId)
    {
        var section = await _context.Sections
            .Include(s => s.Categories)
            .ThenInclude(c => c.Subcategories)
            .FirstOrDefaultAsync(s => s.Id == sectionId);

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
}