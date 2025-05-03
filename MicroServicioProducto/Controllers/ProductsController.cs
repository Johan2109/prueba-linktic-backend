using Microsoft.AspNetCore.Mvc;
using MicroServicioProducto.Models;
using MicroServicioProducto.Services;
using MicroServicioProducto.JsonApi;

namespace MicroServicioProducto.Controllers
{
    // 🎯 Indica que esta clase es un controlador de API
    [ApiController]
    [Route("api/[controller]")] // URL base: /api/products
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        // 💉 Inyección del servicio de productos
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // 📄 GET: /api/products?page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            // Obtiene los productos paginados y el total de elementos
            var (products, totalCount) = await _productService.GetPagedAsync(page, pageSize);

            // Respuesta con formato JSON:API
            var jsonApiResponse = new JsonApiResponse<List<JsonApiData<Product>>>
            {
                Data = products.Select(p => new JsonApiData<Product>
                {
                    Type = "products",
                    Id = p.Id,
                    Attributes = p
                }).ToList(),
                Meta = new JsonApiMeta
                {
                    TotalItems = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                }
            };

            return Ok(jsonApiResponse);
        }

        // 📄 GET: /api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                // Devuelve error 404 en formato JSON:API si no se encuentra
                return NotFound(new JsonApiError
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = $"Product with ID {id} not found"
                });
            }

            var jsonApiResponse = new JsonApiResponse<JsonApiData<Product>>
            {
                Data = new JsonApiData<Product>
                {
                    Type = "products",
                    Id = product.Id,
                    Attributes = product
                }
            };

            return Ok(jsonApiResponse);
        }

        // 📤 POST: /api/products
        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            await _productService.CreateAsync(product);

            var jsonApiResponse = new JsonApiResponse<JsonApiData<Product>>
            {
                Data = new JsonApiData<Product>
                {
                    Type = "products",
                    Id = product.Id,
                    Attributes = product
                }
            };

            // Devuelve 201 Created con enlace al nuevo recurso
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, jsonApiResponse);
        }

        // ✏️ PUT: /api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Product updatedProduct)
        {
            var existing = await _productService.GetByIdAsync(id);

            if (existing == null)
            {
                return NotFound(new JsonApiError
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = $"Product with ID {id} not found"
                });
            }

            updatedProduct.Id = id;
            await _productService.UpdateAsync(id, updatedProduct);

            // Devuelve 204 No Content si se actualiza correctamente
            return NoContent();
        }

        // ❌ DELETE: /api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new JsonApiError
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = $"Product with ID {id} not found"
                });
            }

            await _productService.DeleteAsync(id);

            // Devuelve 204 No Content si se elimina correctamente
            return NoContent();
        }
    }
}
