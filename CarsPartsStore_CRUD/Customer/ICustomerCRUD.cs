using CarsPartsStore_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public interface ICustomerCRUD
    {
        public void AddCustomer(string name, int age, string address); // Add (One Record)
        public void AddCustomers(List<Customer> customers); // Add (Multi Records)
        public void UpdateCustomer(int id, string name = "default", int age = -1, string address = "default"); // Update specific field by id
        public void DeleteCustomer(int id); // Delete

        public void GetCustomerById(int id); // Get (One Record) By ID
        public void GetCustomerByName(string name); // Get (Collection) By Model
        public void GetAllCustomers(); // Get Table (All Records)
    }
}
