using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloomLevelAPI.Models
{
    public class RegistroBoncheo
    {
        public int Id { get; set; }

        [Required]
        public int SesionId { get; set; }

        [ForeignKey("SesionId")]
        public Sesion? Sesion { get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Grado { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public bool EsCorrecto { get; set; }
        public int Puntaje { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
