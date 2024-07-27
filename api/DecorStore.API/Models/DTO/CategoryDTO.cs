public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<CategoryDto> Categories { get; set; }
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<SubcategoryDto> Subcategories { get; set; }
}

public class SubcategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
}