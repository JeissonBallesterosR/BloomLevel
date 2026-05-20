using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloomLevelAPI.Models
{
    public class Sesion
    {
        public int Id { get; set; }

        [Required]
        public int JugadorId { get; set; }

        [ForeignKey("JugadorId")]
        public Jugador? Jugador { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFin { get; set; }
        public int PuntajeTotal { get; set; } = 0;
        public bool Completada { get; set; } = false;

        public ICollection<RegistroClasificacion> Clasificaciones { get; set; } = new List<RegistroClasificacion>();
        public ICollection<RegistroBoncheo> Boncheos { get; set; } = new List<RegistroBoncheo>();
        public ICollection<RegistroEmpaque> Empaques { get; set; } = new List<RegistroEmpaque>();
    }
}
