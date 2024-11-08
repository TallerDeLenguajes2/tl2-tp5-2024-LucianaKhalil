using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class PresupuestoController : ControllerBase
{
    private readonly PresupuestoRepositorio _presupuestoRepository;
    private readonly ProductoRepositorio _productoRepository;

    public PresupuestoController()
    {
        _presupuestoRepository = new PresupuestoRepositorio(@"Data Source=db\Tienda.db;Cache=Shared");
        _productoRepository = new ProductoRepositorio(@"Data Source=db\Tienda.db;Cache=Shared");
    }

     //POST/api/Presupuesto: Permite crear un Presupuesto.
    [HttpPost]
    public ActionResult CreatePresupuesto([FromBody] Presupuesto presupuesto)
    {
        if (presupuesto.NombreDestinatario == "")
        {
            return BadRequest("No se paso el nombre del destinario.");
        }
        _presupuestoRepository.Create(presupuesto);
        return CreatedAtAction(nameof(GetPresupuesto), new { id = presupuesto.IdPresupuesto }, presupuesto);
    }
    // POST/api/Presupuesto/{id}/ProductoDetalle: Permite agregar un Producto existente y una cantidad al presupuesto

    [HttpPut("{id}/ProductoDetalle")]
    public ActionResult AddProduct(int id, [FromForm] int idP, [FromForm] int cantidad)
    {
        Productos producto = _productoRepository.GetById(idP);
        if (producto == null)
        {
            return NotFound($"Producto {idP} no encontrado.");
        }

        _presupuestoRepository.Update(id, producto, cantidad);
        return Ok(_presupuestoRepository.GetById(id));
    }
    // GET/api/presupuesto: Permite listar los presupuestos existentes.
    [HttpGet]
    public ActionResult GetPresupuestos()
    {
        List<Presupuesto> presupuestos = _presupuestoRepository.getAll();
        return Ok(presupuestos);
    }
    // GET/api/Presupuesto/{id}: Permite agregar un Producto existente y una cantidad al presupuesto
    [HttpGet("{id}")]
    public ActionResult GetPresupuesto(int id)
    {
        Presupuesto presupuesto = _presupuestoRepository.GetById(id);
        if (presupuesto == null) NotFound($"No fue encontrado el presupuesto {id}.");
        return Ok(presupuesto);
    }

}