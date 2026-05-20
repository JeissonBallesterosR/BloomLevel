using BloomLevelAPI.Data;
using BloomLevelAPI.DTOs;
using BloomLevelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/jugadores")]
    [Tags("👤 Jugadores")]
    public class JugadoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public JugadoresController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Registra un nuevo jugador</summary>
        [HttpPost]
        public async Task<IActionResult> CrearJugador([FromBody] CrearJugadorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jugador = new Jugador
            {
                Nombre = dto.Nombre,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Jugadores.Add(jugador);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerJugador), new { id = jugador.Id }, new
            {
                jugador.Id,
                jugador.Nombre,
                jugador.FechaRegistro
            });
        }

        /// <summary>Obtiene todos los jugadores registrados</summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerJugadores()
        {
            var jugadores = await _context.Jugadores
                .Select(j => new
                {
                    j.Id,
                    j.Nombre,
                    j.FechaRegistro,
                    TotalSesiones = j.Sesiones.Count
                })
                .ToListAsync();

            return Ok(jugadores);
        }

        /// <summary>Obtiene un jugador por ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerJugador(int id)
        {
            var jugador = await _context.Jugadores
                .Include(j => j.Sesiones)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jugador == null)
                return NotFound(new { mensaje = $"Jugador con ID {id} no encontrado" });

            return Ok(new
            {
                jugador.Id,
                jugador.Nombre,
                jugador.FechaRegistro,
                TotalSesiones = jugador.Sesiones.Count,
                MejorPuntaje = jugador.Sesiones.Any() ? jugador.Sesiones.Max(s => s.PuntajeTotal) : 0
            });
        }

        /// <summary>Obtiene el reporte de un jugador con estadísticas detalladas</summary>
        [HttpGet("{id}/reporte")]
        public async Task<IActionResult> ObtenerReporte(int id)
        {
            var jugador = await _context.Jugadores
                .Include(j => j.Sesiones)
                    .ThenInclude(s => s.Clasificaciones)
                .Include(j => j.Sesiones)
                    .ThenInclude(s => s.Boncheos)
                .Include(j => j.Sesiones)
                    .ThenInclude(s => s.Empaques)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (jugador == null)
                return NotFound(new { mensaje = $"Jugador con ID {id} no encontrado" });

            var sesionesCompletadas = jugador.Sesiones.Where(s => s.Completada).ToList();

            return Ok(new
            {
                Jugador = new { jugador.Id, jugador.Nombre },
                TotalSesiones = jugador.Sesiones.Count,
                SesionesCompletadas = sesionesCompletadas.Count,
                MejorPuntaje = sesionesCompletadas.Any() ? sesionesCompletadas.Max(s => s.PuntajeTotal) : 0,
                PuntajePromedio = sesionesCompletadas.Any() ? (int)sesionesCompletadas.Average(s => s.PuntajeTotal) : 0,
                EstadisticasClasificacion = new
                {
                    TotalAcciones = jugador.Sesiones.SelectMany(s => s.Clasificaciones).Count(),
                    Aciertos = jugador.Sesiones.SelectMany(s => s.Clasificaciones).Count(c => c.EsCorrecto),
                    PuntajeTotal = jugador.Sesiones.SelectMany(s => s.Clasificaciones).Sum(c => c.Puntaje)
                },
                EstadisticasBoncheo = new
                {
                    TotalAcciones = jugador.Sesiones.SelectMany(s => s.Boncheos).Count(),
                    Aciertos = jugador.Sesiones.SelectMany(s => s.Boncheos).Count(b => b.EsCorrecto),
                    PuntajeTotal = jugador.Sesiones.SelectMany(s => s.Boncheos).Sum(b => b.Puntaje)
                },
                EstadisticasEmpaque = new
                {
                    TotalCajasEmpacadas = jugador.Sesiones.SelectMany(s => s.Empaques).Sum(e => e.CajasEmpacadas),
                    PuntajeTotal = jugador.Sesiones.SelectMany(s => s.Empaques).Sum(e => e.Puntaje)
                }
            });
        }
    }
}
