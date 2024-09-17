using CsvHelper;
using MarketMonitorApp.Entities;
using System.Globalization;

namespace MarketMonitorApp.Services
{
    public interface IFileWriter
    {
        void WriteToFile(string path, IEnumerable<Product> products);
    }
    public class FileWriter : IFileWriter
    {
        public void WriteToFile(string path, IEnumerable<Product> products)
        {
            using (var writer = new StreamWriter(path))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(products);
            }
        }
    }
}
