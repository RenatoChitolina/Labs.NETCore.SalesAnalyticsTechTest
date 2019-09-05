using SalesAnalytics.Domain.Interfaces.ApplicationServices;
using SalesAnalytics.Domain.Interfaces.Repositories;
using System.Linq;

namespace SalesAnalytics.Application.Services
{
    public class OrderAnalysisService : IOrderAnalysisService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISalesmanRepository _salesmanRepository;
        private readonly IOrderAnalysisRepository _orderAnalysisRepository;

        public OrderAnalysisService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            ISalesmanRepository salesmanRepository,
            IOrderAnalysisRepository orderAnalysisRepository
        )
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _salesmanRepository = salesmanRepository;
            _orderAnalysisRepository = orderAnalysisRepository;
        }

        public void Analyze()
        {
            var countCustomers = _customerRepository.GetCustomers()
                .Distinct()
                .Count();

            var countSalesmen = _salesmanRepository.GetSalesmen()
                .Distinct()
                .Count();

            var orders = _orderRepository
                .GetOrders()
                .ToList();

            var mostValuableOrder = orders
                .OrderByDescending(o => o.Total)
                .FirstOrDefault();

            var worstPerformingSalesman = orders
                .GroupBy(o => o.Salesman)
                .Select(g => new
                {
                    salesman = g.Key,
                    yield = g.Sum(o => o.Total)
                })
                .OrderBy(a => a.yield)
                .FirstOrDefault()
                ?.salesman;

            _orderAnalysisRepository.GenerateAnalysis(
                countCustomers,
                countSalesmen,
                mostValuableOrder,
                worstPerformingSalesman
            );
        }
    }
}
