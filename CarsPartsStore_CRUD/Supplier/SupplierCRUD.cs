using CarsPartsStore_Context;
using CarsPartsStore_Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public class SupplierCRUD : ISupplierCRUD
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////// Add

        /////////////////////////////////////// Add (One Record)
        public void AddSupplier(string name, string address)
        {
            try
            {
                using (MyDbContext context = new MyDbContext())
                {
                    var supplier = new Supplier { Name = name, Address = address }; // انشاء كائن جديد كل مرة تستدعى الدالة و اضافته للجدول

                    context.Suppliers.Add(supplier);
                    context.SaveChanges();
                }
            }catch (Exception ex) { Console.WriteLine(ex.Message);}
          

        }
        /////////////////////////////////////// Add (Multi Records)
        public void AddSuppliers(List<Supplier> suppliers)
        {
            using (MyDbContext context = new MyDbContext())
            {
                context.Suppliers.AddRange(suppliers);
                context.SaveChanges();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Update specific field by id

        public void UpdateSupplier(int id, string name = "default", string address = "default")
        {
            using (MyDbContext context = new MyDbContext())
            {
                var supplier = context.Suppliers
                     .Where(s => s.IsActive == true)
                     .FirstOrDefault(s => s.SupplierId == id);

                if (supplier != null) // في حال وجد السجل ضمن الجدول
                {
                    if (name != "default")      // في حال تم تمرير اسم جديد
                        supplier.Name = name;
                    if (address != "default")   // في حال تم تمرير عنوان جديد
                        supplier.Address = address;

                    context.SaveChanges();
                    Console.WriteLine($"Updated The Supplier (ID: {id})");
                }
                else // في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Updating The Supplier (ID: {id}), It Doesn't Exist");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Delete

        public void DeleteSupplier(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var supplier = context.Suppliers
                    .FirstOrDefault(s => s.SupplierId == id); //نجلب (السجل – السجلات) ثم نقوم بتعديل قيمة خاصية (IsActive = false)

                if (supplier != null) // في حال وجد السجل ضمن الجدول
                {
                    supplier.IsActive = false;
                    context.SaveChanges();

                    Console.WriteLine($"Deleted The Supplier (ID: {id}), (Name: {supplier.Name})");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Deleting The Supplier (ID: {id}), It Already Doesn't Exist");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Get

        /////////////////////////////////////// Get (One Record) By ID
        public void GetSupplierById(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var supplier = context.Suppliers
                    .Include(s => s.Parts)
                    .Where(s => s.IsActive == true)
                    .FirstOrDefault(c => c.SupplierId == id);

                if (supplier != null) // في حال وجد السجل ضمن الجدول
                {
                    Console.WriteLine($"Supplier:| ID: {supplier.SupplierId} | Name: {supplier.Name} | Address: {supplier.Address}");
                    foreach (var part in supplier.Parts)
                    {
                        Console.WriteLine($"Part:| ID: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} |");
                    }
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Reading The Supplier (ID: {id}), It Doesn't Exist");
                }
            }
        }


        /////////////////////////////////////// Get (Collection) By Name

        public void GetSupplierByName(string name)
        {
            using (MyDbContext context = new MyDbContext())
            {
                List<Supplier> suppliers = context.Suppliers
                    .Include(s => s.Parts)
                    .Where(s => s.IsActive == true && s.Name == name)
                    .ToList();

                if (suppliers.Count != 0)
                {
                    foreach (var supplier in suppliers)
                    {
                        Console.WriteLine($"Supplier:| ID: {supplier.SupplierId} | Name: {supplier.Name} | Address: {supplier.Address}");
                        foreach (var part in supplier.Parts)
                        {
                            Console.WriteLine($"Part:| ID: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} |");
                            Console.WriteLine("ـــــــــــــــــــــ");
                        }
                        Console.WriteLine("===============================================================");
                    }
                }
                else { Console.WriteLine($"Failed Reading The Supplier (Name: {name}), There aren't any Supplier Has this Name"); }
            }
        }

        /////////////////////////////////////// Get Table (All Records)

        // قراءة جميع الموردين غير المحذوفين مع القطع المنتمية اليهم
        public void GetAllSuppliers()
        {
            using (MyDbContext context = new MyDbContext())
            {
                try
                {
                    List<Supplier> suppliers = context.Suppliers
                    .Include(s => s.Parts)
                    .Where(s => s.IsActive)
                    .ToList(); // جلب جميع سجلات جدول الموردين مع ما ينتمي لكل مورد من جدول القطع

                    if (suppliers.Count != 0) // في حال وجد سجلات ضمن الجدول
                    {
                        foreach (var supplier in suppliers) // عملية القراءة من القائمة و الطباعة
                        {
                            Console.WriteLine($"Supplier:| ID: {supplier.SupplierId} | Name: {supplier.Name} | Address: {supplier.Address}");
                            Console.WriteLine("ـــــــــــــــــــــ");
                            foreach (var part in supplier.Parts)
                            {
                                Console.WriteLine($"Part:| ID: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} |");
                            }
                            Console.WriteLine("===============================================================");
                        }
                    }
                    else // في حال لم يجد اي سجل ضمن الجدول
                    { Console.WriteLine("Failed Reading, There aren't any (Supplier) in the (Supplier Table)"); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
