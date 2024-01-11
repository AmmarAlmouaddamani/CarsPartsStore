using CarsPartsStore_CRUD;
using CarsPartsStore_Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
namespace CarsPartsStore_App
{
    internal class Program
    {
        static void Main(string[] args)
        {


            //*******************************Suppliers Json*******************************"
            // لم يعمل هذا الكود لذلك لم اكمل التجربة لباقي الموديلز
            //var suppliersText = File.ReadAllText("Supplier.json");
            //var suppliers = JsonSerializer.Deserialize<Supplier>(suppliersText);





            Console.WriteLine("*******************************العمليات على المورد*******************************");

            SupplierCRUD supplierCRUD = new SupplierCRUD(); // انشاء اوبجيكت للعمليات على المورد
            //supplierCRUD.AddSupplier("Samer","Sarmada");
            //supplierCRUD.AddSuppliers();
            //supplierCRUD.UpdateSupplier(100,"Sameer","Azaz");
            //supplierCRUD.DeleteSupplier(100);
            //supplierCRUD.GetSupplierById(1);
            //supplierCRUD.GetSupplierByName("Mohamad");
            supplierCRUD.GetAllSuppliers();

            Console.WriteLine("*******************************العمليات على القطعة*******************************");

            PartCRUD partCRUD = new PartCRUD(); // انشاء اوبجيكت للعمليات على القطعة
            //partCRUD.AddPart("Hand Break", 50, 4, 1);
            //partCRUD.AddParts(parts);
            //partCRUD.UpdatePart(2,name: "Break");
            //partCRUD.DeletePart(1);
            //partCRUD.GetPartById(1);
            //partCRUD.GetPartByName("Break");
            partCRUD.GetAllParts();

            Console.WriteLine("*******************************العمليات على السيارة*******************************");

            CarCRUD carCRUD = new CarCRUD(); // انشاء اوبجيكت للعمليات على السيارة
            //carCRUD.AddCar("GMC", 2017, "Manual", 25000, 1, 2, 4, 7,8);
            //carCRUD.AddCars(cars); // يوجد مشكلة هنا في القيود
            //carCRUD.UpdateCar(3,"Honda",2008);
            //carCRUD.DeleteCar(3);
            //carCRUD.GetCarById(14);
            //carCRUD.GetCarByModel("Mercedes");
            carCRUD.GetAllCars();

            Console.WriteLine("*******************************العمليات على الزبون*******************************");

            CustomerCRUD customerCRUD = new CustomerCRUD(); // انشاء اوبجيكت للعمليات على الزبون
            //customerCRUD.AddCustomer("Malek", 44, "Hizree");
            //customerCRUD.AddCustomers(customers);
            //customerCRUD.UpdateCustomer(3,"Ahmad",address:"Daer Alzour");
            //customerCRUD.DeleteCustomer(4);
            //customerCRUD.GetCustomerById(4);
            //customerCRUD.GetCustomerByName("Ahmad");
            customerCRUD.GetAllCustomers();

            Console.WriteLine("*******************************العمليات على الصفقة*******************************");

            SaleCRUD SaleCRUD = new SaleCRUD(); // انشاء اوبجيكت للعمليات على صفقة البيع
            //SaleCRUD.AddSale(4,4);
            //SaleCRUD.AddSales(sales);
            //SaleCRUD.UpdateSale(4,7,7);
            //SaleCRUD.DeleteSale(4);
            //SaleCRUD.GetSaleById(2);
            //SaleCRUD.GetSaleByCustomerName("Ahmad");
            SaleCRUD.GetAllSales();





            //// إنشاء قائمة من الموردين
            //var suppliers = new List<Supplier>()
            //{
            //    new Supplier { Name = "Mohamad", Address = "Damascus" },
            //    new Supplier { Name = "Salem", Address = "Aleppo" },
            //    new Supplier { Name = "Mohamad", Address = "Hama" },
            //    new Supplier { Name = "Omar", Address = "Latakia" },
            //    new Supplier { Name = "Ahmad", Address = "Idlib" },
            //};

            //// إنشاء قائمة من القطع
            //var parts = new List<Part>()
            //{
            //    new Part { Name = "Hand Brake", Price = 40, Quantity = 25, SupplierID = 1 },
            //    new Part { Name = "Mirror ", Price = 60.5, Quantity = 50, SupplierID = 5 },
            //    new Part { Name = "Battery", Price = 300, Quantity = 30, SupplierID = 3 },
            //    new Part { Name = "Engine", Price = 5000, Quantity = 20, SupplierID = 6 },
            //    new Part { Name = "Lamp", Price = 110, Quantity = 300, SupplierID = 4 },
            //};

            //// إنشاء قائمة من السيارات
            //var cars = new List<Car>()
            //{
            //    new Car { Model = "Toyota", Year = 2023, Gear = "Automatic", Km = 1000, Parts = new List<Part>(){new Part { Name = "Hand Brake", Price = 60, Quantity = 25, SupplierID = 1 } } },
            //    new Car { Model = "Hyundai", Year = 2022, Gear = "Manual", Km = 5000 ,Parts = new List<Part>(){new Part { Name = "Gear ", Price = 200, Quantity = 50, SupplierID = 5 } }},
            //    new Car { Model = "Kia", Year = 2021, Gear = "Automatic", Km = 3000 , Parts = new List < Part >() { new Part { Name = "Dore", Price = 400, Quantity = 30, SupplierID = 3 } }},
            //    new Car { Model = "Ford", Year = 2020, Gear = "Manual", Km = 4000 ,Parts = new List<Part>(){new Part { Name = "Window", Price = 150, Quantity = 20, SupplierID = 6 } }},
            //    new Car { Model = "Nissan", Year = 2019, Gear = "Automatic", Km = 2000, Parts = new List<Part>(){new Part { Name = "chair", Price = 110, Quantity = 33, SupplierID = 4 } } },
            //};

            //// إنشاء قائمة من العملاء
            //var customers = new List<Customer>()
            //{
            //    new Customer { Name = "Ahmed", Age = 25, Address = "Idlib" },
            //    new Customer { Name = "Sara", Age = 28, Address = "Aleppo" },
            //    new Customer { Name = "Ali", Age = 35, Address = "Damascus" },
            //    new Customer { Name = "Lina", Age = 32, Address = "Homs" },
            //    new Customer { Name = "Omar", Age = 30, Address = "Daraa" },
            //};

            //// إنشاء قائمة من الصفقات
            //var sales = new List<Sale>()
            //{
            //    new Sale { CarId = 1, CustomerId = 1 },
            //    new Sale { CarId = 3, CustomerId = 2 },
            //    new Sale { CarId = 7, CustomerId = 3 },
            //    new Sale { CarId = 5, CustomerId = 4 },
            //    new Sale { CarId = 2, CustomerId = 10 },
            //};
        }
    }
}
