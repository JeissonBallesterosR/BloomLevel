using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloomLevelAPI.Models
{
    public class RegistroEmpaque
    {
        public int Id { get; set; }

        [Required]
        public int SesionId { get; set; }

        [ForeignKey("SesionId")]
        public Sesion? Sesion { get; set; }

        public int CajasEmpacadas { get; set; }
        public int Puntaje { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    }
}
