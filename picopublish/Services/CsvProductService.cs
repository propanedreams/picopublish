namespace picopublish.Services
{
    using CsvHelper;
    using CsvHelper.Configuration;
    using picopublish.Models;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class CsvProductService
    {
        private readonly string _filePath;

        public CsvProductService(string filePath)
        {
            _filePath = filePath;
            if (!File.Exists(_filePath))
            {
                File.Create(_filePath).Close(); // Opret filen, hvis den ikke findes
            }
        }

        public int CountItemsInCsv()
        {
           

            // Read all lines in the file
            var lines = File.ReadAllLines(_filePath);


            return lines.Length;
        }

        public IEnumerable<Product> GetProducts(int page, int pageSize)
        {
            var products = ReadProductsFromFile();
            return products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<int> getAllProductIds()
        {
            var products = ReadProductsFromFile();
            var productIds = products.Select(p => p.Id).ToList();
            return productIds;
        }

        public void AddProduct(Product product)
        {
            var products = ReadProductsFromFile().ToList();
            product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);
            WriteProductsToFile(products);
        }

        public int Count()
        {
            return ReadProductsFromFile().Count();
        }

        private IEnumerable<Product> ReadProductsFromFile()
        {
            using (var reader = new StreamReader(_filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true, // Ensure that CsvHelper knows there's a header record
            }))
            {
                // Skip empty lines or lines that don't conform to the expected format.
                csv.Context.TypeConverterOptionsCache.GetOptions<int>().NullValues.Add(string.Empty);
                csv.Context.TypeConverterOptionsCache.GetOptions<string>().NullValues.Add(string.Empty);

                return csv.GetRecords<Product>().ToList();
            }
        }

        private void WriteProductsToFile(IEnumerable<Product> products)
        {
            using (var writer = new StreamWriter(_filePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(products);
            }
        }
        private void SaveProducts(IEnumerable<Product> products)
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
            csv.WriteRecords(products);
        }

        public Product GetProductById(int id)
        {
            int max = CountItemsInCsv();
            var products = GetProducts(1, max);
            return products.FirstOrDefault(p => p.Id == id);
        }
        public bool UpdateProduct(Product updatedProduct)
        {
            int max = CountItemsInCsv();
            if (updatedProduct == null)
            {
                throw new ArgumentNullException(nameof(updatedProduct));
            }

            var products = GetProducts(1, max)?.ToList();
            if (products == null)
            {
                return false;
            }

            var existingProduct = products.FirstOrDefault(p => p.Id == updatedProduct.Id);
            if (existingProduct == null)
            {
                return false;
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Description = updatedProduct.Description;

            SaveProducts(products);
            return true;
        }


    }

}
