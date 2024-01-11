using CarsPartsStore_Domain;
using Microsoft.EntityFrameworkCore;

namespace CarsPartsStore_Context
{
    public class MyDbContext:DbContext
    {
        // تمرير عنوان الاتصال (اسم السيرفر, اسم القاعدة, معلومات التحقق لتسجيل الدخول) بالقاعدة لمزود القاعدة لانشاء اتصال بها
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString.sqlConnStr);
        }

        // تمثيل الموديل لجداول في القاعدة
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarPart> CarsParts { get; set; }
        public DbSet<Customer>Customers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }



        // تعريف العلاقات بين الجداول
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // تعريف العلاقة الكثير إلى كثير بين السيارات والقطع
            modelBuilder.Entity<Car>()                                              //الكائن سيارة
                .HasMany(c => c.Parts)                                     // لديه العديد من القطع
                .WithMany(p => p.Cars)                     //كما ان القطعة تتبع للعديد من السيارات
                .UsingEntity<CarPart>()         //وبالتالي استخدم هذا الكائن لتمثيل العلاقة السابقة
                .Property(cp => cp.CreatedDate).HasDefaultValueSql("getDate()"); // و اجعل لخاصيته هذه القيمة الافتراضية الناتجة عن هذه الدالة

            //    // تعريف العلاقة الواحد إلى كثير بين الموردين والقطع
            //    modelBuilder.Entity<Supplier>()
            //        .HasMany(s => s.Parts)
            //        .WithOne(p => p.Supplier)
            //        .HasForeignKey(p => p.SupplierID);

            //    // تعريف العلاقة الواحد إلى واحد بين السيارات والمبيعات
            //    modelBuilder.Entity<Car>()
            //        .HasOne(c => c.Sales)
            //        .WithOne(s => s.Car);


            //    // تعريف العلاقة الواحد إلى كثير بين العملاء والمبيعات
            //    modelBuilder.Entity<Customer>()
            //        .HasMany(c => c.Sales)
            //        .WithOne(s => s.Customer)
            //        .HasForeignKey(s => s.CustomerId);
        }
    }
}
