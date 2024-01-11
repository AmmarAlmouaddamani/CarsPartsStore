using CarsPartsStore_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public interface ISaleCRUD
    {
        public void AddSale(int carId, int customerId); // Add (One Record)
        public void AddSales(List<Sale> sales); // Add (Multi Records)
        public void UpdateSale(int id, int carId = -1, int customerId = -1); // Update specific field by id
        public void DeleteSale(int id); // Delete

        public void GetSaleById(int id); // Get (One Record) By ID
        public void GetSaleByCustomerName(string customerName); // Get (Collection) By CustomerName
        public void GetAllSales(); // Get Table (All Records)
    }
}
