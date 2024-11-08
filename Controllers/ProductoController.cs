using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductoController : ControllerBase
{
     private readonly ProductoRepositorio _productoRepository;

     public ProductoController()
    {
        _productoRepository = new ProductoRepositorio(@"Data Source=db\Tienda.db;Cache=Shared");
    }
    // GET/api/Producto: Permite listar los Productos existentes---------------------------------------------
    [HttpGet]
    public ActionResult GetProductos()
    {
        List<Productos> productos = _productoRepository.getAll();
        return Ok(productos);
    }
    // POST/api/Producto: Permite crear un nuevo Producto.----------------------------------------------------
    [HttpGet("{idProducto}")]
    public ActionResult<Productos> GetById(int idProducto)
    {
        Productos producto = _productoRepository.GetById(idProducto);  // Pasar idProducto
        if (producto == null)
        {
            return NotFound();
        }
        return Ok(producto);
    }
    [HttpPost]
    public ActionResult CreateProduct([FromBody] Productos producto)
    {
        if (string.IsNullOrEmpty(producto.Descripcion) || producto.Precio <= 0)
        {
            return BadRequest("Producto inválido. Verifica que la descripción no esté vacía y el precio sea mayor a 0.");
        }

        _productoRepository.Create(producto);
        return CreatedAtAction(nameof(GetById), new { idProducto = producto.IdProducto }, producto);  //idProducto asegurarse que tenga mismo nombre
    }

    //PUT/api/Producto/{Id}: Permite modificar un nombre de un Producto
    [HttpPut("{idProducto}")]
    public ActionResult UpdateProduct([FromBody] Productos producto, int idProducto)
    {
        // Actualiza el producto usando idProducto
        _productoRepository.Update(producto, idProducto);
        return Ok(_productoRepository.GetById(idProducto)); 
    }

}