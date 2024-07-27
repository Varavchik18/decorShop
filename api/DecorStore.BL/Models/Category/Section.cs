using System.ComponentModel.DataAnnotations.Schema;

[Table("Sections_tb", Schema = "Product.Category")]
public class Section
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Category> Categories { get; set; }
}