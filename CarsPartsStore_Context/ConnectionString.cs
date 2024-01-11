using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CarsPartsStore_Context
{
    internal static class ConnectionString
    {
      public const string sqlConnStr = "Server=(localdb)\\MSSQLLocalDB;Database=CarsPartsDb";
    }
}
