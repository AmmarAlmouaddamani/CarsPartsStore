using CarsPartsStore_Context;
using CarsPartsStore_Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public class PartCRUD : IPartCRUD
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////// Add

        /////////////////////////////////////// Add (One Record)
        
        public void AddPart(string name, double price, int quantity, int supplierId)
        {
            try 
            {
                using (MyDbContext context = new MyDbContext())
                {
                    // انشاء كائن جديد عند كل استدعاء للدالة و اضافته للجدول
                    var part = new Part { Name = name, Price = price, Quantity = quantity, SupplierID = supplierId }; 
                   
                    //التحقق من وجود المورد ذو (الرقم) الممرر حيث من غير المنطقي ادخال قطعة ليست مرتبطة بمورد
                    var supplier = context.Suppliers 
                        .Where(s => s.IsActive)
                        .FirstOrDefault(s => s.SupplierId == supplierId);

                    if (supplier != null) // في حال المورد موجود يتم اضافة القطعة لجدول القطع
                    {
                        context.Parts.Add(part);
                        context.SaveChanges();

                        Console.WriteLine($"Success Add Part: ({part.Name}), Supplier: ({supplier.Name})");
                    }
                    else // في حال المورد غير موجود لا يتم اضافة القطعة
                    {
                        Console.WriteLine($"Failed!, Supplier who Has (ID: {supplierId}) not Exist, Can't be Add the Part: ({part.Name}), No related With An Supplier");
                    }
                }
            } 
            catch (Exception ex) { Console.WriteLine(ex.Message);}
        }

        /////////////////////////////////////// Add (Multi Records)

        public void AddParts(List<Part> parts)
        {
            try 
            {
                using (var context = new MyDbContext())
                {
                    foreach (var part in parts)
                    {
                        //التحقق من وجود المورد ذو (الرقم) الممرر حيث من غير المنطقي ادخال قطعة ليست مرتبطة بمورد
                        var supplier = context.Suppliers
                            .Where(s => s.IsActive)
                            .FirstOrDefault(s => s.SupplierId == part.SupplierID);

                        if (supplier != null) // في حال المورد موجود يتم اضافة القطعة لجدول القطع
                        {
                            context.Parts.Add(part);
                            context.SaveChanges();

                            Console.WriteLine($"Success Add Part: ({part.Name}), Supplier: ({supplier.Name})");
                        }
                        else // في حال المورد غير موجود لا يتم اضافة القطعة
                        {
                            Console.WriteLine($"Failed!, Supplier who Has (ID: {part.SupplierID}) not Exist, Can't be Add the Part: ({part.Name}), No related With An Supplier");
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Update specific field by id

        public void UpdatePart(int id, string name = "default", double price = -1, int quantity = -1, int supplierId = -1)
        {
            using (var context = new MyDbContext())
            {
                var part = context.Parts // جلب السجل المراد تعديله ان وجد
                    .Where(p => p.IsActive == true)
                    .FirstOrDefault(p => p.PartId == id);
                if (part != null)
                {
                    if (name != "default") // في حال تم تمرير اسم جديد
                        part.Name = name;
                    if(price != -1)
                        part.Price = price;
                    if(quantity != -1)
                        part.Quantity = quantity;
                    if (supplierId != -1)
                    {
                        //التحقق من وجود المورد ذو (الرقم) الممرر حيث من غير المنطقي ادخال قطعة ليست مرتبطة بمورد
                        var supplier = context.Suppliers
                            .Where(s => s.IsActive)
                            .FirstOrDefault(s => s.SupplierId == supplierId);

                        if (supplier != null)
                            part.SupplierID = supplierId;
                        else
                            Console.WriteLine($"Can't Update Current SupplierId: ({part.SupplierID}) To ({supplierId}), Not Exist ");
                    }

                    context.SaveChanges();
                    Console.WriteLine($"Updated The Part (ID: {id})");
                }
                else // في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Updating The Part (ID: {id}), It Doesn't Exist"); 
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Delete
        
        public void DeletePart(int id)
        {
            using( var context = new MyDbContext())
            {
                var part = context.Parts
                    .Where (p => p.IsActive == true)
                    .FirstOrDefault (p => p.PartId == id);

                if(part != null) // في حال وجد السجل ضمن الجدول
                {
                    part.IsActive = false;
                    context.SaveChanges();

                    Console.WriteLine($"Deleted The Part (ID: {id}), (Name: {part.Name})");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Deleting The Part (ID: {id}), It Already Doesn't Exist");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Get

        /////////////////////////////////////// Get (One Record) By ID

        public void GetPartById(int id)
        {
            using(var context = new MyDbContext())
            {
                var part = context.Parts
                    .Include(p => p.Supplier)
                    .Where(part => part.IsActive == true)
                    .FirstOrDefault(p => p.PartId == id);

                if ( part != null) // في حال وجد السجل ضمن الجدول
                {
                    Console.WriteLine($"Part:| Id: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} | Supplier: {part.Supplier.Name}");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Reading The Part (ID: {id}), It Doesn't Exist");
                }
            }
        }

        /////////////////////////////////////// Get (Collection) By Name
       
        public void GetPartByName(string name)
        {
            using(MyDbContext context = new MyDbContext())
            {
                List<Part> parts= context.Parts
                    .Include(p => p.Supplier)
                    .Where(p => p.IsActive == true && p.Name == name)
                    .ToList();

                if (parts.Count != 0)
                {
                    foreach (var part in parts)
                    {
                        Console.WriteLine($"Part:| Id: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} | Supplier: {part.Supplier.Name}");
                    }
                }
                else { Console.WriteLine($"Failed Reading The Part (Name: {name}), There aren't any Part Has this Name"); }
            }
        }

        /////////////////////////////////////// Get Table (All Records)

        // قراءة جميع القطع غير المحذوفة

        public void GetAllParts()
        {
            using (MyDbContext context = new MyDbContext())
            {
                List<Part> parts = context.Parts
                    .Include(p => p.Supplier)
                    .Where (p => p.IsActive == true)
                    .ToList();

                if( parts.Count != 0) // في حال وجد سجلات ضمن الجدول
                {
                    foreach (var part in parts) // عملية القراءة من القائمة و الطباعة
                    {
                        Console.WriteLine($"Part:| Id: {part.PartId} | Name: {part.Name} | Price: {part.Price} | Quantity: {part.Quantity} | Supplier: {part.Supplier.Name}");
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                }
                else // في حال لم يجد اي سجل ضمن الجدول
                { Console.WriteLine("Failed Reading, There aren't any (Part) in the (Parts Table)"); }
            }

        }
    }
}
