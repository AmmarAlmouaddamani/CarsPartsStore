using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarsPartsStore_Domain
{
    // تمثيل العلاقة عديد لعديد بين السيارة و القطع
    public class CarPart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CarPartId {  get; set; }
        public int CarId { get; set; } // FK
        public Car Car { get; set; }

        public int PartId { get; set; } // FK
        public Part Part { get; set; }

        public DateTime CreatedDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
