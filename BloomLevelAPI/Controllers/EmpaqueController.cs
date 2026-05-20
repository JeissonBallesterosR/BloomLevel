using BloomLevelAPI.Data;
using BloomLevelAPI.DTOs;
using BloomLevelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/empaque")]
    [Tags("📦 Empaque")]
    public class EmpaqueController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmpaqueController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Registra una acción de empaque durante la sesión</summary>
        [HttpPost]
        public async Task<IActionResult> RegistrarEmpaque([FromBody] RegistrarEmpaqueDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _context.Sesiones.FindAsync(dto.SesionId);
            if (sesion == null)
                return NotFound(new { mensaje = $"Sesión con ID {dto.SesionId} no encontrada" });

            if (sesion.Completada)
                return BadRequest(new { mensaje = "No se pueden registrar acciones en una sesión finalizada" });

            var registro = new RegistroEmpaque
            {
                SesionId = dto.SesionId,
                CajasEmpacadas = dto.CajasEmpacadas,
                Puntaje = dto.Puntaje,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Empaques.Add(registro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                registro.Id,
                registro.SesionId,
                registro.CajasEmpacadas,
                registro.Puntaje,
                registro.FechaRegistro,
                mensaje = "Empaque registrado correctamente"
            });
        }

        /// <summary>Obtiene las configuraciones de cajas disponibles para empacar</summary>
        [HttpGet("cajas")]
        public IActionResult ObtenerCajas()
        {
            var cajas = new[]
            {
                new { Id = 1, Nombre = "Caja Pequeña",  CapacidadRamos = 5,  CapacidadTallos = 50,  PuntajeBase = 20, Codigo = "CJ-P01" },
                new { Id = 2, Nombre = "Caja Mediana",  CapacidadRamos = 10, CapacidadTallos = 100, PuntajeBase = 45, Codigo = "CJ-M02" },
                new { Id = 3, Nombre = "Caja Grande",   CapacidadRamos = 20, CapacidadTallos = 200, PuntajeBase = 100, Codigo = "CJ-G03" },
                new { Id = 4, Nombre = "Caja Premium",  CapacidadRamos = 5,  CapacidadTallos = 25,  PuntajeBase = 60, Codigo = "CJ-PR04" }
            };

            return Ok(cajas);
        }
    }
}
