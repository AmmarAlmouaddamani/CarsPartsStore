using CarsPartsStore_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public interface ISupplierCRUD
    {
        public void AddSupplier(string name, string address); // Add (One Record)
        public void AddSuppliers(List<Supplier> suppliers); // Add (Multi Records)
        public void UpdateSupplier(int id, string name = "default", string address = "default"); // Update specific field by id
        public void DeleteSupplier(int id); // Delete

        public void GetSupplierById(int id); // Get (One Record) By ID
        public void GetSupplierByName(string name); // Get (Collection) By Model
        public void GetAllSuppliers(); // Get Table (All Records)
    }
}
