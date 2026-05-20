using BloomLevelAPI.Data;
using BloomLevelAPI.DTOs;
using BloomLevelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/clasificacion")]
    [Tags("🌿 Clasificación")]
    public class ClasificacionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClasificacionController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Registra una acción de clasificación de flores durante la sesión</summary>
        [HttpPost]
        public async Task<IActionResult> RegistrarClasificacion([FromBody] RegistrarClasificacionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _context.Sesiones.FindAsync(dto.SesionId);
            if (sesion == null)
                return NotFound(new { mensaje = $"Sesión con ID {dto.SesionId} no encontrada" });

            if (sesion.Completada)
                return BadRequest(new { mensaje = "No se pueden registrar acciones en una sesión finalizada" });

            var registro = new RegistroClasificacion
            {
                SesionId = dto.SesionId,
                TipoFlor = dto.TipoFlor,
                Rango = dto.Rango,
                EsCorrecto = dto.EsCorrecto,
                Puntaje = dto.Puntaje,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Clasificaciones.Add(registro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                registro.Id,
                registro.SesionId,
                registro.TipoFlor,
                registro.Rango,
                registro.EsCorrecto,
                registro.Puntaje,
                registro.FechaRegistro,
                mensaje = "Clasificación registrada correctamente"
            });
        }

        /// <summary>Obtiene los rangos de clasificación disponibles en el juego</summary>
        [HttpGet("rangos")]
        public async Task<IActionResult> ObtenerRangos()
        {
            var rangos = await _context.RangosClasificacion
                .OrderByDescending(r => r.PuntajeMin)
                .ToListAsync();

            return Ok(rangos);
        }
    }
}
