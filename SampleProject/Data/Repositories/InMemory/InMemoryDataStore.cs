using BusinessEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Repositories.InMemory
{
    public class InMemoryDataStore
    {
        private static readonly Lazy<InMemoryDataStore> _instance = new Lazy<InMemoryDataStore>(() => new InMemoryDataStore());

        public static InMemoryDataStore Instance => _instance.Value;

        public List<Product> Products { get; private set; }
        public List<Order> Orders { get; private set; }

        private InMemoryDataStore()
        {
            Products = new List<Product>();
            Orders = new List<Order>();
        }
    }
}
