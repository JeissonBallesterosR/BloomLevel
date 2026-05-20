using System.ComponentModel.DataAnnotations;

namespace BloomLevelAPI.Models
{
    public class Jugador
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        public ICollection<Sesion> Sesiones { get; set; } = new List<Sesion>();
    }
}
