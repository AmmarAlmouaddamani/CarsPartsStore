using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace CarsPartsStore_Domain
{
    public class Sale
    {
        // تعريف الخصائص الأساسية للمبيعة
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SaleId { get; set; } // معرف المبيعة
        public double Total { get; set; }// سعر المبيعة الكلي 
        public int CarId { get; set; } // FK  // معرف السيارة التي تم بيعها
        public int CustomerId { get; set; } // FK // معرف العميل الذي قام بالشراء
        public bool IsActive { get; set; } = true; // for soft delete

        // تعريف العلاقات مع الجداول الأخرى
        public Car Car { get; set; } // السيارة التي تم بيعها
        public Customer Customer { get; set; } // العميل الذي قام بالشراء

        
    }
}
