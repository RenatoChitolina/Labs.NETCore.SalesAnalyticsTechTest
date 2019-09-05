using SalesAnalytics.Domain.Entities;
using System.Collections.Generic;

namespace SalesAnalytics.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        IEnumerable<Order> GetOrders();
    }
}
