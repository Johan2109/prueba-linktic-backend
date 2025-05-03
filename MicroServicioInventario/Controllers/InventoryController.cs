using Microsoft.AspNetCore.Mvc;
using MicroServicioInventario.Models;
using MicroServicioInventario.Services;
using MicroServicioInventario.JsonApi;

namespace MicroServicioInventario.Controllers
{
    // Marca esta clase como un controlador de API.
    [ApiController]
    // Define la ruta base del controlador: api/inventory
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        // Servicio inyectado para manejar la lógica del inventario
        private readonly InventoryService _inventoryService;

        public InventoryController(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // GET: api/inventory
        // Obtiene todos los registros de inventario y los devuelve en formato JSON:API
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _inventoryService.GetAllAsync();

            var response = new JsonApiResponse<Inventory>
            {
                Data = items.Select(i => new JsonApiData<Inventory>
                {
                    Type = "inventory",
                    Id = i.Id!,
                    Attributes = i,
                    Relationships = new
                    {
                        product = new
                        {
                            data = new
                            {
                                type = "products",
                                id = i.ProductId
                            }
                        }
                    }
                }).ToList()
            };

            return Ok(response);
        }

        // GET: api/inventory/{productId}
        // Obtiene el inventario correspondiente a un producto por su ID
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetByProductId(string productId)
        {
            var item = await _inventoryService.GetByProductIdAsync(productId);
            if (item is null)
                return NotFound(new { error = "Inventory item not found." });

            var response = new JsonApiResponseSingle<Inventory>
            {
                Data = new JsonApiData<Inventory>
                {
                    Type = "inventory",
                    Id = item.Id!,
                    Attributes = item,
                    Relationships = new
                    {
                        product = new
                        {
                            data = new
                            {
                                type = "products",
                                id = item.ProductId
                            }
                        }
                    }
                }
            };

            return Ok(response);
        }

        // POST: api/inventory
        // Crea un nuevo registro de inventario
        [HttpPost]
        public async Task<IActionResult> Create(Inventory inventory)
        {
            await _inventoryService.CreateAsync(inventory);

            var response = new JsonApiResponseSingle<Inventory>
            {
                Data = new JsonApiData<Inventory>
                {
                    Type = "inventory",
                    Id = inventory.Id!,
                    Attributes = inventory,
                    Relationships = new
                    {
                        product = new
                        {
                            data = new
                            {
                                type = "products",
                                id = inventory.ProductId
                            }
                        }
                    }
                }
            };

            // Devuelve 201 Created con la ubicación del nuevo recurso
            return CreatedAtAction(nameof(GetByProductId), new { productId = inventory.ProductId }, response);
        }

        // PUT: api/inventory/{productId}
        // Actualiza el stock disponible para un producto específico
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateStock(string productId, [FromBody] int newStock)
        {
            var updated = await _inventoryService.UpdateStockAsync(productId, newStock);
            return updated ? NoContent() : NotFound(new { error = "Inventory item not found." });
        }

        // DELETE: api/inventory/{productId}
        // Elimina el registro de inventario de un producto
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteByProductId(string productId)
        {
            var deleted = await _inventoryService.DeleteByProductIdAsync(productId);
            return deleted
                ? NoContent()
                : NotFound(new { error = $"No inventory found for productId: {productId}" });
        }
    }
}