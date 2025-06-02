using BusinessEntities;
using Core.Services.Users;
using Data.Repositories.InMemory;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
//using System.Web.Mvc;
using WebApi.Models.Users;

namespace WebApi.Controllers
{
    public class OrdersController : ApiController
    {
        private readonly InMemoryDataStore _store = InMemoryDataStore.Instance;

        // GET api/orders
        public IHttpActionResult Get(string customerName = null)
        {
            var orders = _store.Orders;

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                var filtered = orders
                    .Where(p => p.CustomerName.IndexOf(customerName, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                return Ok(filtered);
            }

            return Ok(orders);
        }

        // GET api/orders/{id}
        public HttpResponseMessage Get(Guid id)
        {
            var order = _store.Orders.FirstOrDefault(p => p.Id == id);
            if (order == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound, "Order Not Found");
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, order);
        }

        // POST api/orders
        public HttpResponseMessage Post([FromBody] Order order)
        {
            if (order == null) 
                return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound, "Order Not Found");

            order.Id = Guid.NewGuid();
            _store.Orders.Add(order);
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/orders/{id}
        public HttpResponseMessage Put(Guid id, [FromBody] Order updatedOrder)
        {
            var existing = _store.Orders.FirstOrDefault(p => p.Id == id);
            if (existing == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);

            existing.CustomerName = updatedOrder.CustomerName;
            existing.OrderDate = updatedOrder.OrderDate;
            existing.ProductIds = updatedOrder.ProductIds;

            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/orders/{id}
        public HttpResponseMessage Delete(Guid id)
        {
            var order = _store.Orders.FirstOrDefault(p => p.Id == id);
            if (order == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);

            _store.Orders.Remove(order);
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/orders/filter")]
        public HttpResponseMessage Filter(string customerName = null, DateTime? orderDate = null, decimal? maxPrice = null)
        {
            var orders = _store.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(customerName))
            {
                orders = orders.Where(p => p.CustomerName != null && p.CustomerName.IndexOf(customerName, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (orderDate.HasValue)
            {
                // Convert incoming date to UTC for comparison
                var utcOrderDate = DateTime.SpecifyKind(orderDate.Value, DateTimeKind.Local).ToUniversalTime();
                orders = orders.Where(p => p.OrderDate.Date == utcOrderDate.Date);
            }

            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, orders.ToList());
        }
    }

}