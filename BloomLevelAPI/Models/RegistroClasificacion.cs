using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloomLevelAPI.Models
{
    public class RegistroClasificacion
    {
        public int Id { get; set; }

        [Required]
        public int SesionId { get; set; }

        [ForeignKey("SesionId")]
        public Sesion? Sesion { get; set; }

        [MaxLength(100)]
        public string TipoFlor { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Rango { get; set; } = string.Empty;

        public bool EsCorrecto { get; set; }
        public int Puntaje { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
