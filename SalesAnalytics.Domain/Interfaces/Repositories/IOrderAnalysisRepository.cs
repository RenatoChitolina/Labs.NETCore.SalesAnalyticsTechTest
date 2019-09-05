using SalesAnalytics.Domain.Entities;

namespace SalesAnalytics.Domain.Interfaces.Repositories
{
    public interface IOrderAnalysisRepository
    {
        void GenerateAnalysis(
            int countCustomers, 
            int countSalesmen, 
            Order mostValuableOrder, 
            Salesman worstPerformingSalesman
        );
    }
}
