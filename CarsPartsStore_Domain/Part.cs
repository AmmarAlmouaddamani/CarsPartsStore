using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsPartsStore_Domain
{
    public class Part
    {
        // تعريف الخصائص الأساسية للقطعة
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PartId { get; set; } // معرف القطعة
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; } // اسم القطعة
        [Required]
        [Range(0.5,1000)]
        public double Price { get; set; } // سعر القطعة
        [Required]
        public int Quantity { get; set; } // كمية القطعة
        public int SupplierID { get; set; } // FK // معرف المورد الذي يوفر القطعة
        public bool IsActive { get; set; } = true; // for soft delete

        // تعريف العلاقات مع الجداول الأخرى
        public Supplier Supplier { get; set; } // 1:M المورد الذي يوفر القطعة
        public List<Car> Cars { get; set; } = new List<Car>(); // M:M// قائمة السيارات التي تحتوي على القطعة
    }
}
