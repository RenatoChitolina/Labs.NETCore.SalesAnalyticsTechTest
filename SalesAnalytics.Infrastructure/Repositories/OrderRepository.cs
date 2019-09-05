using SalesAnalytics.Domain.Entities;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Domain.Interfaces.Repositories;
using System.Collections.Generic;

namespace SalesAnalytics.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbContext _dbContext;

        public OrderRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Order> GetOrders()
        {
            return _dbContext.Orders;
        }
    }
}
