using BloomLevelAPI.Data;
using BloomLevelAPI.DTOs;
using BloomLevelAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/sesiones")]
    [Tags("🎮 Sesiones")]
    public class SesionesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SesionesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Inicia una nueva sesión de juego para un jugador</summary>
        [HttpPost("iniciar")]
        public async Task<IActionResult> IniciarSesion([FromBody] IniciarSesionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var jugador = await _context.Jugadores.FindAsync(dto.JugadorId);
            if (jugador == null)
                return NotFound(new { mensaje = $"Jugador con ID {dto.JugadorId} no encontrado" });

            var sesion = new Sesion
            {
                JugadorId = dto.JugadorId,
                FechaInicio = DateTime.UtcNow,
                PuntajeTotal = 0,
                Completada = false
            };

            _context.Sesiones.Add(sesion);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                sesion.Id,
                sesion.JugadorId,
                NombreJugador = jugador.Nombre,
                sesion.FechaInicio,
                sesion.Completada,
                mensaje = "Sesión iniciada correctamente"
            });
        }

        /// <summary>Finaliza una sesión de juego y guarda el puntaje final</summary>
        [HttpPut("finalizar")]
        public async Task<IActionResult> FinalizarSesion([FromBody] FinalizarSesionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sesion = await _context.Sesiones.FindAsync(dto.SesionId);
            if (sesion == null)
                return NotFound(new { mensaje = $"Sesión con ID {dto.SesionId} no encontrada" });

            if (sesion.Completada)
                return BadRequest(new { mensaje = "La sesión ya fue finalizada" });

            sesion.FechaFin = DateTime.UtcNow;
            sesion.PuntajeTotal = dto.PuntajeTotal;
            sesion.Completada = true;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                sesion.Id,
                sesion.JugadorId,
                sesion.FechaInicio,
                sesion.FechaFin,
                sesion.PuntajeTotal,
                sesion.Completada,
                DuracionSegundos = (int)(sesion.FechaFin.Value - sesion.FechaInicio).TotalSeconds,
                mensaje = "Sesión finalizada correctamente"
            });
        }
    }
}
