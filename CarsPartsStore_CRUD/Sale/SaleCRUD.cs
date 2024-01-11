using Azure;
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
    public class SaleCRUD : ISaleCRUD
    {
        ///////////////////////////// Tuple Method دالة التحقق من وجود السيارة
        // هي دالة ترجع قيمتين (قيمة بوليانية تخص وجود السيارة في جدول السيارات , قيمة سعر السيارة ان وجدت)و
        private (bool isCarExist , double total) ValidateCar(int carId)
        {
            using(MyDbContext DB = new MyDbContext())
            {
                var car = DB.Cars // جلب السيارة
                    .Include(c => c.Parts)
                    .Where(c => c.IsActive == true)
                    .FirstOrDefault(c => c.CarId == carId);

                if (car != null) // في حال انها موجودة يتم حساب سعر القطع المضافة لها
                {
                    var total = 0.0;
                    foreach(var part in car.Parts)
                    {
                        total +=  part.Price;
                    }
                    Console.WriteLine($"The Car ({carId}), (Model: {car.Model}) Ready to Add to Sale");
                    return (true, total);
                }
                else // في حال غير موجودة تخبر بذلك
                {
                    Console.WriteLine($"Failed Add The Car ({carId}) to Sale, It Doesn't Exist, First Add It and Try Again");
                    return (false, 0);
                }
            }
        }

        ///////////////////////////// دالة التحقق من وجود الزبون
        // هي دالة ترجع قيمة (قيمة بوليانية تخص وحود الزبون في جدول الزبائن)و
        private bool ValidateCustomer(int customerId)
        {
            using (MyDbContext DB = new MyDbContext())
            {
                var customer = DB.Customers // جلب الزبون
                    .Where(c => c.IsActive == true)
                    .FirstOrDefault(c => c.CustomerId == customerId);

                if (customer != null) // في حال انه موجودة ترجعه
                {
                    Console.WriteLine($"The Customer ({customerId}), (Name: {customer.Name}) Ready to Add to Sale");
                    return true;
                }
                else // في حال غير موجود تخبر بذلك
                {
                    Console.WriteLine($"Failed Reading The Customer ({customerId}), Doesn't Exist, First Add Him and Try Again");
                    return false;
                }
            }
        }
    

        ////////////////////////////////////////////////////////////////////////////////////////////////// Add

        /////////////////////////////////////// Add (One Record)

        public void AddSale(int carId, int customerId)
        {
            try
            {
                using (MyDbContext context = new MyDbContext())
                {
                    // استدعاء دالة التحقق من وجود السيارة و ارجاع نتيجة التحقق مع سعر السيارة ان وجدت
                    var carResult = ValidateCar(carId);
                    // استدعاء دالة التحقق من وجود الزبون
                    var IsCustomerExist = ValidateCustomer(customerId); 

                    // حتى تتم صفقة (بيعة) لا بد من وجود الزبون و السيارة و الا فلن تتم الصفقة
                    if (carResult.isCarExist && IsCustomerExist)
                    {
                        // انشاء صفقة جديدة و اضافتها لجدول الصفقات
                        var sale = new Sale { Total = carResult.total, CarId = carId, CustomerId = customerId };
                        context.Sales.Add(sale);
                        context.SaveChanges();

                        //حذف السيارة من جدول السيارات لانها تباع مرة واحدة فقط, وتغيير حالتها لمباعة
                        var car = context.Cars // جلب سجل السيارة من جدول السيارات
                            .Include(c => c.Sales) // مع سجل الصفقة المرتبطة بها من جدول البيعات 
                            .ThenInclude(s => s.Customer) // مع سجل الزبون المرتبط بها من جدول الزبائن 
                            .FirstOrDefault(c => c.CarId == carId); // للسيارة التي تحقق هذا الشرط

                            car.IsActive = false;
                            car.IsSale = true;
                        
                        context.SaveChanges();
                        Console.WriteLine($"Success Add Sale: ({sale.SaleId}), Car ({car.Model}) For The Customer ({car.Sales.Customer.Name})");
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                    else 
                    {
                        Console.WriteLine($"Failed Add Sale: Car ({carId}) For The Customer ({customerId})");
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                }
            }catch (Exception ex) { Console.WriteLine(ex.Message); }
            
        }

        /////////////////////////////////////// Add (Multi Records)
        
        public void AddSales(List<Sale> sales)
        {
            using (MyDbContext context = new MyDbContext())
            {
                // استدعاء دالة اضافة (صفقة واحدة) من اجل كل صفقة ممررة ضمن قائمة الصفقات
                foreach (var sale in sales)
                {
                    AddSale(sale.CarId, sale.CustomerId);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Update specific field by id

        public void UpdateSale(int id, int carId = -1, int customerId = -1)
        {
            using (MyDbContext context = new MyDbContext())
            {
                // جلب الصفقة من جدول الصفقات ان وجدت
                var sale = context.Sales
                     .Where(s => s.IsActive == true)
                     .FirstOrDefault(s => s.SaleId == id);

                if (sale != null) // في حال وجدت الصفقة
                {
                    //تخزين رقم السيارة و الزبون الحاليين قبل اعتماد التعديل عليهما 
                    var currentCarId = sale.CarId;
                    var currentCustomerId = sale.CustomerId;

                    // في حال نريد التعديل لسيارة مختلفة
                    if (carId != -1)
                    {
                        // اولا نتأكد من وجود السيارة الجديدة باستدعاء دالة التحقق من وجود السيارة
                        var carResult = ValidateCar(carId);
                        if (carResult.isCarExist) // في حال انها موجودة وقبل اعتمادها بدلا من الحالية
                        {
                            // نرجع اعدادات السيارة الحالية لما كانت عليه قبل ادخالها في الصفقة الماضية
                            var currentCar = context.Cars.FirstOrDefault(c => c.CarId == currentCarId);
                            currentCar.IsActive = true; // التراجع عن حذفها
                            currentCar.IsSale = false; // التراجع عن بيعها

                            //نحذف السيارة الجديدة و نعدل حالتها لمباعة
                            var newCar = context.Cars.FirstOrDefault(c => c.CarId == carId);
                            newCar.IsActive = false; // التراجع عن حذفها
                            newCar.IsSale = true; // التراجع عن بيعها

                            // الان نعدل الصفقة بالسيارة الجديدة و السعر الجديد
                            sale.CarId = carId;
                            sale.Total = carResult.total;
                            context.SaveChanges();
                        }
                    }

                    if (customerId != -1)
                    {
                        // اولا نتأكد من وجود الزبون الجديد باستدعاء دالة التحقق من وجود الزبون
                        var IsCustomerExist = ValidateCustomer(customerId);
                        if (IsCustomerExist) // اذا تحقق وجوده ضمن جدول الزبائن نعتمد التعديل
                        {
                            sale.CustomerId = customerId;
                            context.SaveChanges();
                        }
                    }
                    Console.WriteLine($"Updated The Sale ({id})");
                    Console.WriteLine($"Old Car: ({currentCarId}), Old Customer: ({currentCustomerId})");
                    Console.WriteLine($"New Car: ({sale.CarId}), New Customer: ({sale.CustomerId})");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else // في حال الصفقة محذوفة او غير مضافة للجدول
                {
                    Console.WriteLine($"Failed Updating The Sale (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Delete

        public void DeleteSale(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var sale = context.Sales
                    .Include(s =>s.Car)
                    .Include(s => s.Customer)
                    .Where(s => s.IsActive)
                    .FirstOrDefault(s => s.SaleId == id); //نجلب السجل

                if (sale != null) // في حال وجد السجل ضمن الجدول
                {
                    // تعديل السيارة لغير مباعة و غير محذوفة
                    sale.Car.IsSale= false;
                    sale.Car.IsActive = true;
                    // حذف الصفقة
                    sale.IsActive = false;
                    context.SaveChanges();

                    Console.WriteLine($"Deleted The Sale: ({sale.SaleId}) | Car: ({sale.Car.CarId}) ({sale.Car.Model}) | Customer: ({sale.Customer.CustomerId}) ({sale.Customer.Name}) | Total: ({sale.Total}) |");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Deleting The Sale (ID: {id}), It Already Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Get

        /////////////////////////////////////// Get (One Record) By ID
        public void GetSaleById(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var sale = context.Sales
                    .Include(s => s.Car)
                    .Include(s => s.Customer)
                    .Where(s => s.IsActive == true)
                    .FirstOrDefault(s => s.SaleId == id);

                if (sale != null) // في حال وجد السجل ضمن الجدول
                {
                    Console.WriteLine($"Sale: ({sale.SaleId}) | Car: ({sale.Car.CarId}) ({sale.Car.Model}) | Customer: ({sale.Customer.CustomerId}) ({sale.Customer.Name}) | Total: ({sale.Total}) |");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Reading The Sale (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }


        /////////////////////////////////////// Get (Collection) By CustomerName

        public void GetSaleByCustomerName(string customerName)
        {
            using (MyDbContext context = new MyDbContext())
            {
                List<Sale> sales = context.Sales
                    .Include(s => s.Car)
                    .Include(s => s.Customer)
                    .Where(s => s.IsActive == true && s.Customer.Name == customerName)
                    .ToList();

                if (sales.Count != 0)
                {
                    foreach (var sale in sales)
                    {
                        Console.WriteLine($"Sale: ({sale.SaleId}) | Car: ({sale.Car.CarId}) ({sale.Car.Model}) | Customer: ({sale.Customer.CustomerId}) ({sale.Customer.Name}) | Total: ({sale.Total}) |");
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                }
                else 
                {
                    Console.WriteLine($"Failed Reading The Sales for (Customer: {customerName}), There aren't any Sale Related Him!!");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        /////////////////////////////////////// Get Table (All Records)

        // قراءة جميع الزبائن غير المحذوفين مع الصفقات (المبيعات) المنتمية اليهم
        public void GetAllSales()
        {
            using (MyDbContext context = new MyDbContext())
            {
                try
                {
                    List<Sale> sales = context.Sales
                        .Include(s => s.Car)
                        .Include(s => s.Customer)
                    .Where(s => s.IsActive)
                    .ToList(); // جلب جميع سجلات جدول الزبائن مع ما ينتمي لكل زبون من جدول المبيعات

                    if (sales.Count != 0) // في حال وجد سجلات ضمن الجدول
                    {
                        foreach (var sale in sales) // عملية القراءة من القائمة و الطباعة
                        {
                           Console.WriteLine($"Sale: ({sale.SaleId}) | Car: ({sale.Car.CarId}) ({sale.Car.Model}) | Customer: ({sale.Customer.CustomerId}) ({sale.Customer.Name}) | Total: ({sale.Total}) |");
                           Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                        }
                    }
                    else // في حال لم يجد اي سجل ضمن الجدول
                    {
                        Console.WriteLine("Failed Reading, There aren't any (Sale) in the (Sale Table)");
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
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
