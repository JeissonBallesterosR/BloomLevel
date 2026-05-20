using BloomLevelAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Controllers
{
    [ApiController]
    [Route("api/ranking")]
    [Tags("🏆 Ranking")]
    public class RankingController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RankingController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>Obtiene el ranking global de jugadores basado en su mejor puntaje</summary>
        [HttpGet]
        public async Task<IActionResult> ObtenerRanking()
        {
            var ranking = await _context.Jugadores
                .Include(j => j.Sesiones)
                .Where(j => j.Sesiones.Any(s => s.Completada))
                .Select(j => new
                {
                    JugadorId = j.Id,
                    NombreJugador = j.Nombre,
                    MejorPuntaje = j.Sesiones
                        .Where(s => s.Completada)
                        .Max(s => s.PuntajeTotal),
                    TotalSesiones = j.Sesiones.Count(s => s.Completada),
                    UltimaPartida = j.Sesiones
                        .Where(s => s.Completada && s.FechaFin != null)
                        .OrderByDescending(s => s.FechaFin)
                        .Select(s => s.FechaFin)
                        .FirstOrDefault()
                })
                .OrderByDescending(r => r.MejorPuntaje)
                .Take(20)
                .ToListAsync();

            var rankingConPosicion = ranking
                .Select((r, index) => new
                {
                    Posicion = index + 1,
                    r.JugadorId,
                    r.NombreJugador,
                    r.MejorPuntaje,
                    r.TotalSesiones,
                    r.UltimaPartida
                })
                .ToList();

            return Ok(rankingConPosicion);
        }
    }
}
