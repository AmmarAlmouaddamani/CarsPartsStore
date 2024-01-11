using CarsPartsStore_Context;
using CarsPartsStore_Domain;

namespace CarsPartsStore_CRUD
{
    public interface ICarCRUD
    {
        public void AddCar(string model, int year, string gear, int km, params int[] partId); // Add (One Record)
        public void AddCars(List<Car> cars); // Add (Multi Records)
        public void UpdateCar(int id, string model = "default", int year = 0, string gear = "default", int km = -1); // Update specific field by id
        public void DeleteCar(int id); // Delete

        public void GetCarById(int id); // Get (One Record) By ID
        public void GetCarByModel(string model); // Get (Collection) By Model
        public void GetAllCars(); // Get Table (All Records)
    }
}
