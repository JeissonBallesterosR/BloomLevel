using System.ComponentModel.DataAnnotations;

namespace BloomLevelAPI.DTOs
{
    public class CrearJugadorDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
    }

    public class IniciarSesionDto
    {
        [Required(ErrorMessage = "El JugadorId es obligatorio")]
        public int JugadorId { get; set; }
    }

    public class FinalizarSesionDto
    {
        [Required(ErrorMessage = "El SesionId es obligatorio")]
        public int SesionId { get; set; }

        public int PuntajeTotal { get; set; }
    }

    public class RegistrarClasificacionDto
    {
        [Required]
        public int SesionId { get; set; }

        [MaxLength(100)]
        public string TipoFlor { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Rango { get; set; } = string.Empty;

        public bool EsCorrecto { get; set; }
        public int Puntaje { get; set; }
    }

    public class RegistrarBoncheoDto
    {
        [Required]
        public int SesionId { get; set; }

        [MaxLength(50)]
        public string Codigo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Grado { get; set; } = string.Empty;

        public int Cantidad { get; set; }
        public bool EsCorrecto { get; set; }
        public int Puntaje { get; set; }
    }

    public class RegistrarEmpaqueDto
    {
        [Required]
        public int SesionId { get; set; }

        public int CajasEmpacadas { get; set; }
        public int Puntaje { get; set; }
    }
}
