using SalesAnalytics.Domain.Entities;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Domain.Interfaces.Repositories;
using System.Collections.Generic;

namespace SalesAnalytics.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbContext _dbContext;

        public CustomerRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            return _dbContext.Customers;
        }
    }
}
