using System.ComponentModel.DataAnnotations.Schema;
namespace DecorStore.BL.Models
{
    [Table("Subcategories_tb", Schema = "Product.Category")]
    public class Subcategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string IconUrl { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}