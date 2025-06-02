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
    public class ProductsController : ApiController
    {
        private readonly InMemoryDataStore _store = InMemoryDataStore.Instance;

        // GET api/products
        public IHttpActionResult Get(string name = null)
        {
            var products = _store.Products;

            if (!string.IsNullOrWhiteSpace(name))
            {
                var filtered = products
                    .Where(p => p.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();
                return Ok(filtered);
            }

            return Ok(products);
        }

        // GET api/products/{id}
        public HttpResponseMessage Get(Guid id)
        {
            var product = _store.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound, "Product Not Found");
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, product);
        }

        // POST api/products
        public HttpResponseMessage Post([FromBody] Product product)
        {
            if (product == null) 
                return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound, "Product Not Found");

            product.Id = Guid.NewGuid();
            _store.Products.Add(product);
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/products/{id}
        public HttpResponseMessage Put(Guid id, [FromBody] Product updatedProduct)
        {
            var existing = _store.Products.FirstOrDefault(p => p.Id == id);
            if (existing == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);

            existing.Name = updatedProduct.Name;
            existing.Price = updatedProduct.Price;

            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        // DELETE api/products/{id}
        public HttpResponseMessage Delete(Guid id)
        {
            var product = _store.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return ControllerContext.Request.CreateResponse(HttpStatusCode.NotFound);

            _store.Products.Remove(product);
            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("api/products/filter")]
        public HttpResponseMessage Filter(string name = null, decimal? minPrice = null, decimal? maxPrice = null)
        {
            var products = _store.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                products = products.Where(p => p.Name != null && p.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            return ControllerContext.Request.CreateResponse(HttpStatusCode.OK, products.ToList());
        }
    }

}