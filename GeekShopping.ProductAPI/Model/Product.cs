using GeekShopping.ProductAPI.Model.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.ProductAPI.Model
{
    [Table("product")]
    public class Product : BaseEntity
    {
        [Column("name")]
        [Required]
        [MaxLength(150)]
        public string? Name { get; set; }

        [Column("price")]
        [Required]
        [Range(1, 10000)]
        public decimal Price { get; set; }

        [Column("description")]
        [StringLength(100)]
        public string? Description { get; set; }

        [Column("category_name")]
        [StringLength(50)]
        public string? Category_Name { get; set;}

        [Column("image_url")]
        [StringLength(300)]
        public string Image_URL { get; set; }

    }
}
