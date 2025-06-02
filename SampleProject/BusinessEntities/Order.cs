using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessEntities
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public List<Guid> ProductIds { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
