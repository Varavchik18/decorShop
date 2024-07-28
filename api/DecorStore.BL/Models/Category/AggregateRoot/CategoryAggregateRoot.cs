using DecorStore.BL.Models;

public class CategoryAggregate
{
    public Section Section { get; private set; }
    public IReadOnlyCollection<Category> Categories => _categories.AsReadOnly();
    public IReadOnlyCollection<Subcategory> Subcategories => _subcategories.AsReadOnly();

    private List<Category> _categories = new List<Category>();
    private List<Subcategory> _subcategories = new List<Subcategory>();

    public CategoryAggregate(Section section)
    {
        Section = section;
    }

    public void AddCategory(Category category)
    {
        _categories.Add(category);
    }

    public void AddSubcategory(Subcategory subcategory)
    {
        _subcategories.Add(subcategory);
    }

    public void RemoveCategory(Category category)
    {
        foreach (var subcategory in category.Subcategories)
        {
            _subcategories.Remove(subcategory);
        }
        _categories.Remove(category);
    }
}