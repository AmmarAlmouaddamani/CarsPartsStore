using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsPartsStore_Domain
{
    public class Car // Entity Set
    {
        // تعريف الخصائص الأساسية للسيارة
        [Key] // PK
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // الترقيم تلقائي في القاعدة
        public int CarId { get; set; } // معرف السيارة
        [Required] // !null
        [Column(TypeName = "varchar(50)")]
        public string Model { get; set; } // نوع السيارة
        [Range(1900,2024)] // التاريخ محصور بين هاتين القيمتين فقط
        public int Year { get; set; } // سنة الصنع
        public string Gear { get; set; } // نوع الغيار
        [Range(0,600000)]
        public int Km { get; set; } // عداد الكيلومتراج
        public bool IsActive { get; set; } = true; // for soft delete
        public bool IsSale { get; set; } = false; // لتحديد حالة بيع السيارة | كل سيارة تباع يجب ان تحذف من الجدول

        // تعريف العلاقات مع الجداول الأخرى
        public Sale Sales { get; set; } // FK // Car 1:1 Sale البيعة التي بيعت فيها السيارة
        public List<Part> Parts { get; set; } = new List<Part>(); // M:M // قائمة القطع التي تنتمي إلى السيارة
    }
}
