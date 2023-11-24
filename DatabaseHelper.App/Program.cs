using DatabaseHelper.MsSql;
using System.Data;

namespace DatabaseHelper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using SqlDatabase database = new("server = DESKTOP-GHNTHT8; database = northwind; integrated security = true; TrustServerCertificate=true;");

            DataTable table = database.GetDataTable("Select * From Products");

            foreach (DataRow dr in table.Rows)
            {
                Console.WriteLine($"{dr["ProductID"]} {dr["ProductName"]}");
            }
            Console.WriteLine();

        }
    }
}