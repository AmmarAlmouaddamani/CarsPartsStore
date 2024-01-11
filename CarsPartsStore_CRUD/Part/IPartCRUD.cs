using CarsPartsStore_Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarsPartsStore_CRUD
{
    public interface IPartCRUD
    {
        public void AddPart(string name, double price, int quantity, int supplierId); // Add (One Record)
        public void AddParts(List<Part> parts); // Add (Multi Records)
        public void UpdatePart(int id, string name = "default", double price = -1, int quantity = -1, int supplierId = -1); // Update specific field by id
        public void DeletePart(int id); // Delete

        public void GetPartById(int id); // Get (One Record) By ID
        public void GetPartByName(string name); // Get (Collection) By Model
        public void GetAllParts(); // Get Table (All Records)
    }
}
