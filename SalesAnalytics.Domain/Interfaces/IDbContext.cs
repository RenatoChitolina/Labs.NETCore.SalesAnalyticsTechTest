using SalesAnalytics.Domain.Entities;
using System.Collections.Generic;

namespace SalesAnalytics.Domain.Interfaces
{
    public interface IDbContext
    {
        HashSet<Customer> Customers { get; }
        HashSet<Salesman> Salesmen { get; }
        HashSet<Order> Orders { get; }

        void Load();
        void Save(string data = null);
    }
}
