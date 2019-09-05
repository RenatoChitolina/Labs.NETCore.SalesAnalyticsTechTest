using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SalesAnalytics.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public ICollection<OrderItem> Items { get; set; }
        public Salesman Salesman { get; set; }
        /*
         * Há uma inconsistência nos requisitos, afinal, não me parece fazer sentido existirem 
         * pedidos sem clientes. Por causa disso, pode acontecer de, se não houver um nome de 
         * cliente vinculado no pedido, essa propriedade ficar nula
         */
        public Customer Customer { get; set; }
        public decimal Total => Items.Sum(i => i.Total);

        public Order()
        {
            Items = new Collection<OrderItem>();
        }
    }
}
