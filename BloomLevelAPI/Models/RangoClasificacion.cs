using System.ComponentModel.DataAnnotations;

namespace BloomLevelAPI.Models
{
    public class RangoClasificacion
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        public string Descripcion { get; set; } = string.Empty;

        public int PuntajeMin { get; set; }
        public int PuntajeMax { get; set; }
        public string Color { get; set; } = string.Empty;
    }
}
