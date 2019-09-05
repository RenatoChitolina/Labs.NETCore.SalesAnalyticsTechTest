using SalesAnalytics.Domain.Entities;
using System.Collections.Generic;

namespace SalesAnalytics.Domain.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetCustomers();
    }
}
