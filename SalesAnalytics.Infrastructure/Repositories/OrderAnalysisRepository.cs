using SalesAnalytics.Domain.Entities;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Domain.Interfaces.Repositories;
using System.Text;

namespace SalesAnalytics.Infrastructure.Repositories
{
    public class OrderAnalysisRepository : IOrderAnalysisRepository
    {
        private readonly IDbContext _dbContext;

        public OrderAnalysisRepository(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void GenerateAnalysis(
            int countCustomers, 
            int countSalesmen, 
            Order mostValuableOrder, 
            Salesman worstPerformingSalesman
        )
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"Customers: {countCustomers}");
            stringBuilder.AppendLine($"Salesmen: {countSalesmen}");
            stringBuilder.AppendLine($"Most Valuable Order ID: {mostValuableOrder.Id}");
            stringBuilder.AppendLine($"Worst Performing Salesman: {worstPerformingSalesman.Name} - {worstPerformingSalesman.CPF}");

            _dbContext.Save(stringBuilder.ToString());
        }
    }
}
