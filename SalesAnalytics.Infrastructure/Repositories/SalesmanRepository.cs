using SalesAnalytics.Domain.Entities;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Domain.Interfaces.Repositories;
using System.Collections.Generic;

namespace SalesAnalytics.Infrastructure.Repositories
{
    public class SalesmanRepository : ISalesmanRepository
    {
        private readonly IDbContext _dbContext;

        public SalesmanRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Salesman> GetSalesmen()
        {
            return _dbContext.Salesmen;
        }
    }
}
