namespace SalesAnalytics.Domain.Entities
{
    public class Customer
    {
        public string CNPJ { get; set; }
        public string Name { get; set; }
        /*
         * Com mais detalhes sobre o negócio, possivelmente essa propriedade seria 
         * extraída para uma nova entidade
         */
        public string BusinessArea { get; set; }
    }
}
