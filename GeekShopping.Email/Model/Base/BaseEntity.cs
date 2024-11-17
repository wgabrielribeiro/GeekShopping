using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GeekShopping.Email.Model.Base
{
    public class BaseEntity
    {
        [Key]
        [Column(name: "id", TypeName = "bigint")]
        public long Id { get; set; }
    }
}
