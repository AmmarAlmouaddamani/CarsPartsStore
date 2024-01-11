using CarsPartsStore_Context;
using CarsPartsStore_Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public class CarCRUD : ICarCRUD
    {
        //private readonly MyDbContext _context; // تعريف كونتكست لاستخدامه ضمن هذا الكلاس فقط
        //public CarCRUD (MyDbContext context) // بعد انشائه وتمريره من خلال الباني
        //{
        //    _context = context;
        //}

        ///////////////////////////////////////// دالة التحقق من وجود قطعة ممرر رقمها في جدول القطع
        // اولا: التحقق من وجود القطع الممررة ارقامها في جدول القطع
        // ثانيا: جمعها القطع الموجودة ضمن قائمة تجهيزاً لربطها مع السيارة 
        // ثالثا: الابلاغ عن القطع الغير موجودة

        private List<Part> partsVerify(int[] partId)
        {

            using (MyDbContext context = new MyDbContext())
            {
                List<Part> parts = new List<Part>(); // مجموعة لتخزين (القطع الموجودة) من القطع الممررة

                foreach (var id in partId) // من اجل رقم كل قطعة ممرر
                {
                    var part = context.Parts  // جلب سجل القطعة في حال رقمها يحقق الرقم الممرر
                      .Where(p => p.IsActive)
                      .FirstOrDefault(p => p.PartId == id);

                    if (part != null)  // في حال ان القطعة متوفرة:
                    {
                        parts.Add(part); // تضاف للقائمة (كنسخة منعزلة عن السجل الحقيقي في الجدول)و 

                        part.Quantity--; // ويتم انقاص كمية القطعة من جدول القطع

                        if (part.Quantity <= 0) // و في حال انتهت كميتها يتم حذفها من جدول القطع
                            part.IsActive = false;

                        context.SaveChanges();
                    }
                    else  // القطعة غير المتوفرة: نخبر بذلك و ننتقل لفحص رقم باراميتر القطعة التالي
                    { Console.WriteLine($"Failed Adding The part ({id}), it Doesn't Exist"); }
                }
                return parts;
            }
        }

        ///////////////////////////////////////// دالة اضافة القطع (الممرة ارقامها) للسيارة في حال كانت متوفرة ضمن جدول القطع 
        // رابعاً: ربط كل قطعة (موجودة) مع السيارة بانشاء اوبجيكت (سجل) جديد لهما و اضافته للجدول الوسيط (سيارة-قطعة) من خلال دالة اضافة القطع للسيارة

        private void AddPartsToCar(int[] partId , int carId)
        {
            using(MyDbContext context = new MyDbContext())
            {
                // استدعاء دالة التحقق من وجود القطع  والحصول على قائمة (نسخة من السجلات اي التعديل عليها لا يؤثر على القاعدة) بالقطع الموجودة
                List<Part> parts = partsVerify(partId); 
                foreach (var part in parts) // من اجل كل قطعة من القائمة
                {
                    var CarPart = new CarPart { CarId = carId , PartId = part.PartId}; // انشاء سجل سيارة-قطعة

                    context.CarsParts.Add(CarPart); // اضافة السجل الجديد لجدول كسر العلاقة
                    
                    context.SaveChanges();

                    Console.WriteLine($"Success Add Part: ({part.Name}) to the Car | New Quantity: ({part.Quantity})");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Add

        /////////////////////////////////////// Add (One Record)
        public void AddCar(string model,int year,string gear,int km, params int [] partId)
        {
            try
            {
                using (MyDbContext context = new MyDbContext())
                {
                    // انشاء كائن جديد عند كل استدعاء للدالة و اضافته للجدول
                    var car = new Car { Model = model, Year = year, Gear = gear, Km = km}; 
                    context.Cars.Add(car);
                    context.SaveChanges(); // تمت اضافة السيارة لجدول السيارات

                    Console.WriteLine($"Preparing The Car: ({car.Model}) ...");

                    AddPartsToCar(partId , car.CarId);  //استدعاء دالة اضافة القطع (المحددة بالمصفوفة الممررة) للسيارة ضمن الجدول المساعد

                    Console.WriteLine($"The Car: ({car.Model}), Is Ready");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }catch (Exception ex) { Console.WriteLine("from AddCar Method" + " " + ex.Message); }
        }
        /////////////////////////////////////// Add (Multi Records)
        public void AddCars(List<Car> cars)
        {
            using (MyDbContext context = new MyDbContext())
            { 
                context.Cars.AddRange(cars);
                context.SaveChanges();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Update specific field by id

        public void UpdateCar(int id, string model = "default", int year = 0, string gear = "default", int km = -1)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var car = context.Cars
                     .Where(c => c.IsActive == true)
                     .FirstOrDefault(c => c.CarId == id);

                if (car != null) // في حال وجد السجل ضمن الجدول
                {
                    if (model != "default") // في حال تم تمرير موديل جديد
                        car.Model = model;
                    if (year != 0)           // في حال تم تمرير سنة صنع جديدة
                        car.Year = year;
                    if (gear != "default")
                        car.Gear = gear;
                    if (km != -1)
                        car.Km = km;

                    context.SaveChanges();
                    Console.WriteLine($"Updated The Car (ID: {id})");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else // في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Updating The Car (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Delete

        public void DeleteCar(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var car = context.Cars
                    .FirstOrDefault(c => c.CarId == id); //نجلب (السجل – السجلات) ثم نقوم بتعديل قيمة خاصية (IsActive = false)

                if (car != null) // في حال وجد السجل ضمن الجدول
                {
                    car.IsActive = false; // حذف السيارة

                    // حذف ارتباط السيارة بالقطع من جدول كسر العلاقة و ارجاع كمية القطعة كما كانت قبل الارتباط (ارجاع القطع للمستودع)و
                    List<CarPart> carPart = context.CarsParts // جميع السجلات ارتباط السيارة بالقطع
                        .Include(cp => cp.Part)
                        .Where(cp => cp.CarId == id)
                        .ToList();
                    foreach (var cp in carPart) // من اجل كل ارتباط
                    {
                        cp.IsActive = false; // احذفه
                        cp.Part.Quantity++;  // و ارجع كمية القطعة كما كانت قبل الارتباط
                    }

                    context.SaveChanges();
                    Console.WriteLine($"Deleted The Car (ID: {id}), (Model: {car.Model})");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Deleting The Car (ID: {id}), It Already Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Get

        /////////////////////////////////////// Get (One Record) By ID
        public void GetCarById(int id)
        {
            using (MyDbContext context = new MyDbContext())
            { 
                var car = context.Cars
                    .Include(c => c.Parts)
                    .ThenInclude(p => p.Supplier) // for: {part.Supplier.Name}
                    .Where(c => c.IsActive == true)
                    .FirstOrDefault(c => c.CarId == id);
                if (car != null) // في حال وجد السجل ضمن الجدول
                {
                    Console.WriteLine($"Car: {car.CarId} | {car.Model} | {car.Year} | {car.Gear} | {car.Km}  KM |");
                    foreach (var part in car.Parts)
                    {
                        Console.WriteLine($"Has Part: ({part.PartId}) | Name: ({part.Name}) | Price: ({part.Price}) | Quantity: (1) | Supplier:({part.Supplier.Name})");
                    }
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Reading The Car (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }


        /////////////////////////////////////// Get (Collection) By Model

        public void GetCarByModel(string model)
        {
           using(MyDbContext context = new MyDbContext())
           {
                List<Car> cars = context.Cars
                    .Include(c => c.Parts)
                    .ThenInclude(p => p.Supplier)
                    .Where(c => c.IsActive == true && c.Model == model)
                    .ToList();

                    if (cars.Count != 0)
                    {
                        foreach (var car in cars)
                        {
                            Console.WriteLine($"Car: {car.CarId} | {car.Model} | {car.Year} | {car.Gear} | {car.Km} KM |");
                            foreach (var part in car.Parts)
                            {
                            Console.WriteLine($"Has Part: ({part.PartId}) | Name: ({part.Name}) | Price: ({part.Price}) | Quantity: (1) | Supplier:({part.Supplier.Name})");
                        }
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                    }
                    else { Console.WriteLine($"Failed Reading The Cars (Model: {model}), There aren't any Car Has this Model");}
           }
        }

        /////////////////////////////////////// Get Table (All Records)

        // قراءة جميع السيارات غير المحذوفة مع القطع المنتمية اليها
        public void GetAllCars()
        {
            using (MyDbContext context = new MyDbContext()) 
            {
                try
                {
                    List<Car> cars = context.Cars
                    .Include(c => c.Parts)
                    .ThenInclude(p => p.Supplier)
                    .Where(c => c.IsActive)
                    .ToList(); // جلب جميع سجلات جدول السيارات مع ما ينتمي لكل منها من جدول القطع

                    if (cars.Count != 0) // في حال وجد سجلات ضمن الجدول
                    { 
                        foreach (var car in cars) // عملية القراءة من القائمة و الطباعة
                        {
                            Console.WriteLine($"Car: {car.CarId} | {car.Model} | {car.Year} | {car.Gear} | {car.Km} KM |");
                            foreach (var part in car.Parts)
                            {
                                Console.WriteLine($"Has Part: ({part.PartId}) | Name: ({part.Name}) | Price: ({part.Price}) | Quantity: (1) | Supplier:({part.Supplier.Name})");
                            }
                            Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                        }
                    }
                    else // في حال لم يجد اي سجل ضمن الجدول
                    { Console.WriteLine("Failed Reading, There aren't any (Car) in the (Cars Table)"); }
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                } 
            }
           
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
