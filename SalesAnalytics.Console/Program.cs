using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalytics.Application.Services;
using SalesAnalytics.Domain.Interfaces;
using SalesAnalytics.Domain.Interfaces.ApplicationServices;
using SalesAnalytics.Domain.Interfaces.Repositories;
using SalesAnalytics.Infrastructure;
using SalesAnalytics.Infrastructure.Configurations;
using SalesAnalytics.Infrastructure.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SalesAnalytics.Console
{
    class Program
    {
        private static IServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            RegisterServices();

            var cancellationTokenSource = new CancellationTokenSource();

            /* A ideia aqui não é escalar a aplicação sob multi-threading, e sim sob contêineres do Docker */
            var task = Task.Run(() => Work(cancellationTokenSource.Token), cancellationTokenSource.Token);

            System.Console.Write("Pressione qualquer tecla para finalizar a aplicação: ");

            System.Console.ReadKey();

            cancellationTokenSource.Cancel();

            System.Console.WriteLine("\nFinalizando... Aguarde um momento...");

            Thread.Sleep(3000);

            DisposeServices();
        }
        
        private static void Work(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<IDbContext>();

                    context.Load();

                    var analysisService = scope.ServiceProvider.GetService<IOrderAnalysisService>();

                    if(context.Orders.Any())
                        analysisService.Analyze();
                }
            }
        }

        private static void RegisterServices()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var collection = new ServiceCollection();

            var dataDirectoriesConfig = LoadConfig<DataDirectoriesConfiguration>(config, "DataDirectories");

            collection.AddSingleton(dataDirectoriesConfig);
            collection.AddScoped<IDbContext, FileContext>();
            collection.AddScoped<IOrderAnalysisService, OrderAnalysisService>();
            collection.AddScoped<IOrderRepository, OrderRepository>();
            collection.AddScoped<ICustomerRepository, CustomerRepository>();
            collection.AddScoped<ISalesmanRepository, SalesmanRepository>();
            collection.AddScoped<IOrderAnalysisRepository, OrderAnalysisRepository>();

            _serviceProvider = collection.BuildServiceProvider();
        }

        private static void DisposeServices()
        {
            if (_serviceProvider == null)
                return;

            if (_serviceProvider is IDisposable)
                ((IDisposable)_serviceProvider).Dispose();
        }

        private static T LoadConfig<T>(IConfiguration config, string section) where T : new()
        {
            var obj = new T();

            config.GetSection(section).Bind(obj);

            return obj;
        }
    }
}
