using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Ecom.Domain.Entities
{
    [Table("ProductTags")]
    public class ProductTag
    {
        [Column("ProductId")]
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        [Column("TagId")]
        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}