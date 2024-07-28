using System.ComponentModel.DataAnnotations.Schema;

namespace DecorStore.BL.Models
{
    [Table("Categories_tb", Schema = "Product.Category")]
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public ICollection<Subcategory> Subcategories { get; set; }
    }
}