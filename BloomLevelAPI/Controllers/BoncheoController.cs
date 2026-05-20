using BloomLevelAPI.Data;
using BloomLevelAPI.DTOs;
using BloomLevelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/boncheo")]
    [Tags("🎉 Boncheo")]
    public class BoncheoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BoncheoController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Registra una acción de boncheo durante la sesión</summary>
        [HttpPost]
        public async Task<IActionResult> RegistrarBoncheo([FromBody] RegistrarBoncheoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _context.Sesiones.FindAsync(dto.SesionId);
            if (sesion == null)
                return NotFound(new { mensaje = $"Sesión con ID {dto.SesionId} no encontrada" });

            if (sesion.Completada)
                return BadRequest(new { mensaje = "No se pueden registrar acciones en una sesión finalizada" });

            var registro = new RegistroBoncheo
            {
                SesionId = dto.SesionId,
                Codigo = dto.Codigo,
                Grado = dto.Grado,
                Cantidad = dto.Cantidad,
                EsCorrecto = dto.EsCorrecto,
                Puntaje = dto.Puntaje,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Boncheos.Add(registro);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                registro.Id,
                registro.SesionId,
                registro.Codigo,
                registro.Grado,
                registro.Cantidad,
                registro.EsCorrecto,
                registro.Puntaje,
                registro.FechaRegistro,
                mensaje = "Boncheo registrado correctamente"
            });
        }

        /// <summary>Obtiene el código de boncheo para un grado específico de flor</summary>
        [HttpGet("codigo/{grado}")]
        public async Task<IActionResult> ObtenerCodigoPorGrado(string grado)
        {
            // Tabla de grados y sus códigos según estándar floricultor
            var codigos = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                { "A",  new { Grado = "A",  Codigo = "FL-A01", Descripcion = "Flor de primera calidad, tallo largo (60-70cm)",  PuntajePorUnidad = 10 } },
                { "B",  new { Grado = "B",  Codigo = "FL-B02", Descripcion = "Flor de segunda calidad, tallo medio (40-60cm)",  PuntajePorUnidad = 7  } },
                { "C",  new { Grado = "C",  Codigo = "FL-C03", Descripcion = "Flor estándar, tallo corto (30-40cm)",             PuntajePorUnidad = 5  } },
                { "D",  new { Grado = "D",  Codigo = "FL-D04", Descripcion = "Flor de descarte, uso decorativo o relleno",       PuntajePorUnidad = 2  } }
            };

            if (!codigos.ContainsKey(grado))
                return NotFound(new { mensaje = $"Grado '{grado}' no encontrado. Grados disponibles: A, B, C, D" });

            return Ok(codigos[grado]);
        }
    }
}
