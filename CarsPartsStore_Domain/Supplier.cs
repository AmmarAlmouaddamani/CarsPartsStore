using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsPartsStore_Domain
{
    public class Supplier
    {
        // تعريف الخصائص الأساسية للمورد
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; } // معرف المورد
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } // اسم المورد
        public string Address { get; set; } // عنوان المورد
        public bool IsActive { get; set; } = true; // for soft delete

        // تعريف العلاقات مع الجداول الأخرى
        public List<Part> Parts { get; set; } = new List<Part>(); // 1:M // قائمة القطع التي يوفرها المورد
    }
}
