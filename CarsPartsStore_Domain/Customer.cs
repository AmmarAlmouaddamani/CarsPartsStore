using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsPartsStore_Domain
{
    public class Customer // Entity Set
    {
        // تعريف الخصائص الأساسية للعميل
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; } // معرف العميل
        [Required]
        [Column(TypeName ="nvarchar(100)")]
        public string Name { get; set; } // اسم العميل
        [Range(18,100)]
        public int Age { get; set; } // عمر العميل
        public string Address { get; set; } // عنوان العميل
        public bool IsActive { get; set; } = true; // for soft delete

        // تعريف العلاقات مع الجداول الأخرى
        public List<Sale> Sales { get; set; } = new List<Sale>(); // 1:M مجموعة الشرائات التابعة للعميل
    }
}
