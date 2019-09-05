using SalesAnalytics.CrossCutting.Extensions;
using SalesAnalytics.Domain.Entities;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Infrastructure.Configurations;
using SalesAnalytics.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SalesAnalytics.Infrastructure
{
    public class FileContext : IDbContext
    {
        private const string HomePathKey = "HOMEPATH";

        private readonly string _inputDirectory;
        private readonly string _outputDirectory;
        private readonly string _timestamp;
        private readonly CultureInfo _decimalConversionCulture = new CultureInfo("en-US");

        public HashSet<Customer> Customers { get; private set; }
        public HashSet<Salesman> Salesmen { get; private set; }
        public HashSet<Order> Orders { get; private set; }

        public FileContext(DataDirectoriesConfiguration dataDirectoriesConfig)
        {
            var baseDirectory = string.Empty;

            if (dataDirectoriesConfig.Relative)
                baseDirectory = Environment.GetEnvironmentVariable(HomePathKey) ?? string.Empty;

            _inputDirectory = Path.Combine(baseDirectory, dataDirectoriesConfig.Input);
            _outputDirectory = Path.Combine(baseDirectory, dataDirectoriesConfig.Output);
            _timestamp = new DateTimeOffset(DateTime.Now)
                .ToUnixTimeMilliseconds()
                .ToString();

            Customers = new HashSet<Customer>();
            Salesmen = new HashSet<Salesman>();
            Orders = new HashSet<Order>();
        }

        public void Load()
        {
            if (!Directory.Exists(_inputDirectory))
                Directory.CreateDirectory(_inputDirectory);

            var files = Directory.GetFiles(_inputDirectory);

            var filteredFiles = files.Where(f => Path.GetExtension(f).Equals(".txt"));

            foreach (var (file, index) in filteredFiles.WithIndex())
            {
                var data = LoadFile(file);

                File.Move(file, Path.Combine(_inputDirectory, $"{_timestamp}_{index}.rem"));

                LoadCollections(data, index == 0);
            }
        }

        public void Save(string data = null)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;

            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);

            SaveFile(Path.Combine(_outputDirectory, $"{_timestamp}.txt"), data);
        }

        private string LoadFile(string path)
        {
            var data = string.Empty;

            using (var stream = new StreamReader(path))
            {
                data = stream.ReadToEnd().Trim();
            }

            return data;
        }

        private void LoadCollections(string data, bool reset = false)
        {
            if (reset)
            {
                Customers.Clear();
                Salesmen.Clear();
                Orders.Clear();
            }

            var lines = Regex.Split(data, "\r\n|\r|\n");

            foreach (var line in lines)
            {
                try
                {
                    var lineWithoutBreak = Regex.Replace(line, "\r\n|\r|\n", string.Empty);

                    var columns = Regex.Split(lineWithoutBreak, "[ç]");

                    var lineType = LineTypeHelper.StringToLineType(columns[0]);

                    switch (lineType)
                    {
                        case Enums.LineType.Customer:
                            FillCustomersCollection(columns);
                            break;
                        case Enums.LineType.Salesman:
                            FillSalesmenCollection(columns);
                            break;
                        case Enums.LineType.Order:
                            FillOrdersCollection(columns);
                            break;
                    }
                }
                catch (Exception)
                {
                    //no momento, o único tratamento será um bypass
                }
            }
        }

        private void FillSalesmenCollection(string[] columns)
        {
            var cpf = columns[1];

            if (Salesmen.FirstOrDefault(s => s.CPF.Equals(cpf)) != null)
                return;

            var salesman = new Salesman()
            {
                CPF = cpf,
                Name = columns[2],
                Salary = decimal.TryParse(columns[3], NumberStyles.Any, _decimalConversionCulture, out decimal salary)
                    ? salary
                    : 0
            };

            Salesmen.Add(salesman);
        }

        private void FillCustomersCollection(string[] columns)
        {
            var cnpj = columns[1];

            if (Customers.FirstOrDefault(c => c.CNPJ.Equals(cnpj)) != null)
                return;

            var customer = new Customer()
            {
                CNPJ = cnpj,
                Name = columns[2],
                BusinessArea = columns[3]
            };

            Customers.Add(customer);
        }

        private void FillOrdersCollection(string[] columns)
        {
            int.TryParse(columns[1], out int id);

            if (id == 0
                || Orders.FirstOrDefault(o => o.Id == id) != null)
                return;

            var order = new Order
            {
                Id = id,
                Salesman = Salesmen.FirstOrDefault(c => c.Name.Equals(columns[3])),
                Customer = columns.Length >= 5
                    ? Customers.FirstOrDefault(c => c.Name.Equals(columns[4]))
                    : null
            };

            var columnWithoutBrackets = Regex.Replace(columns[2], "[[]", string.Empty);
            columnWithoutBrackets = Regex.Replace(columnWithoutBrackets, "[]]", string.Empty);

            var lineItems = Regex.Split(columnWithoutBrackets, "[,]");

            foreach (var lineItem in lineItems)
            {
                var columnItems = Regex.Split(lineItem, "[-]");

                FillOrderItemsCollection(order, columnItems);
            }

            Orders.Add(order);
        }

        private void FillOrderItemsCollection(Order order, string[] columns)
        {
            var orderItem = new OrderItem
            {
                Id = int.TryParse(columns[0], out int id)
                    ? id
                    : 0,
                Quantity = decimal.TryParse(columns[1], NumberStyles.Any, _decimalConversionCulture, out decimal quantity)
                    ? quantity
                    : 0,
                Price = decimal.TryParse(columns[2], NumberStyles.Any, _decimalConversionCulture, out decimal price)
                    ? price
                    : 0
            };

            order.Items.Add(orderItem);
        }

        private void SaveFile(string path, string data)
        {
            using (var stream = File.CreateText(path))
            {
                stream.Write(data);
            }
        }
    }
}
