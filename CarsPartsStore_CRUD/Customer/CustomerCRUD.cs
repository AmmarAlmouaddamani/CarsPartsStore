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
    public class CustomerCRUD : ICustomerCRUD
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////// Add

        /////////////////////////////////////// Add (One Record)
        
        public void AddCustomer(string name,int age,string address)
        {
            try
            {
                using (MyDbContext context = new MyDbContext())
                {
                    var customer = new Customer { Name = name, Age = age, Address = address }; // انشاء كائن جديد كل مرة تستدعى الدالة و اضافته للجدول
                    context.Customers.Add(customer);
                    context.SaveChanges();
                }
            }catch (Exception ex) { Console.WriteLine(ex.Message); }
            

        }

        /////////////////////////////////////// Add (Multi Records)
        
        public void AddCustomers(List<Customer> customers)
        {
            using (MyDbContext context = new MyDbContext())
            {
                context.Customers.AddRange(customers);
                context.SaveChanges();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Update specific field by id

        public void UpdateCustomer(int id, string name = "default",int age = -1, string address = "default")
        {
            using (MyDbContext context = new MyDbContext())
            {
                var customer = context.Customers
                     .Where(c => c.IsActive == true)
                     .FirstOrDefault(c => c.CustomerId == id);

                if (customer != null) // في حال وجد السجل ضمن الجدول
                {
                    if (name != "default")      // في حال تم تمرير اسم جديد
                        customer.Name = name;
                    if (age != -1)              // في حال تمرير عمر جديد
                        customer.Age = age;
                    if (address != "default")   // في حال تم تمرير عنوان جديد
                        customer.Address = address;

                    context.SaveChanges();
                    Console.WriteLine($"Updated The Customer (ID: {id})");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else // في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Updating The Customer (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Delete

        public void DeleteCustomer(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var customer = context.Customers
                    .FirstOrDefault(c => c.CustomerId == id); //نجلب (السجل – السجلات) ثم نقوم بتعديل قيمة خاصية (IsActive = false)

                if (customer != null) // في حال وجد السجل ضمن الجدول
                {
                    customer.IsActive = false;
                    context.SaveChanges();

                    Console.WriteLine($"Deleted The Customer (ID: {id}), (Name: {customer.Name})");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Deleting The Customer (ID: {id}), It Already Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////// Get

        /////////////////////////////////////// Get (One Record) By ID
        public void GetCustomerById(int id)
        {
            using (MyDbContext context = new MyDbContext())
            {
                var customer = context.Customers
                    .Include(c => c.Sales)
                    .ThenInclude(s => s.Car)
                    .Where(c => c.IsActive == true)
                    .FirstOrDefault(c => c.CustomerId == id);

                if (customer != null) // في حال وجد السجل ضمن الجدول
                {
                    Console.WriteLine($"Customer: {customer.CustomerId} | {customer.Name} | {customer.Age} | {customer.Address}");
                    foreach (var sale in customer.Sales.Where(s => s.IsActive)) // جلب البيانات من الصفقات غير المحذوفة فقط
                    {
                        Console.WriteLine($"Sale: ({sale.SaleId}) | Total: ({sale.Total}) | Car: ({sale.Car.Model})");
                    }
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else //  في حال السجل محذوف او غير مضاف للجدول
                {
                    Console.WriteLine($"Failed Reading The Customer (ID: {id}), It Doesn't Exist");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
            }
        }


        /////////////////////////////////////// Get (Collection) By Name

        public void GetCustomerByName(string name)
        {
            using (MyDbContext context = new MyDbContext())
            {
                List<Customer> customers = context.Customers
                    .Include(c => c.Sales)
                    .ThenInclude(s => s.Car)
                    .Where(c => c.IsActive == true && c.Name == name)
                    .ToList();

                if (customers.Count != 0)
                {
                    foreach (var customer in customers)
                    {
                        Console.WriteLine($"Customer: {customer.CustomerId} | {customer.Name} | {customer.Age} | {customer.Address}");
                        foreach (var sale in customer.Sales.Where(s => s.IsActive)) // جلب البيانات من الصفقات غير المحذوفة فقط
                        {
                            Console.WriteLine($"Sale: ({sale.SaleId}) | Total: ({sale.Total}) | Car: ({sale.Car.Model})");
                        }
                    }
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                else 
                { 
                    Console.WriteLine($"Failed Reading The Customer (Name: {name}), There aren't any Customer Has this Name");
                    Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                }
                
            }
        }

        /////////////////////////////////////// Get Table (All Records)

        // قراءة جميع الزبائن غير المحذوفين مع الصفقات (المبيعات) المنتمية اليهم
        public void GetAllCustomers()
        {
            using (MyDbContext context = new MyDbContext())
            {
                try
                {
                    List<Customer> customers = context.Customers
                    .Include(c => c.Sales)
                    .ThenInclude(s => s.Car)
                    .Where(c => c.IsActive)
                    .ToList(); // جلب جميع سجلات جدول الزبائن مع ما ينتمي لكل زبون من جدول المبيعات

                    if (customers.Count != 0) // في حال وجد سجلات ضمن الجدول
                    {
                        foreach (var customer in customers) // عملية القراءة من القائمة و الطباعة
                        {
                            Console.WriteLine($"Customer: {customer.CustomerId} | {customer.Name} | {customer.Age} | {customer.Address}");
                            foreach (var sale in customer.Sales.Where(s => s.IsActive)) // جلب البيانات من الصفقات غير المحذوفة فقط
                            {
                                Console.WriteLine($"Sale: ({sale.SaleId}) | Total: ({sale.Total}) | Car: ({sale.Car.Model})");
                            }
                        }
                        Console.WriteLine("ــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــــ");
                    }
                    else // في حال لم يجد اي سجل ضمن الجدول
                    {
                        Console.WriteLine("Failed Reading, There aren't any (Customer) in the (Customer Table)");
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
