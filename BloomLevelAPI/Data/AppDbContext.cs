using BloomLevelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BloomLevelAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Sesion> Sesiones { get; set; }
        public DbSet<RegistroClasificacion> Clasificaciones { get; set; }
        public DbSet<RegistroBoncheo> Boncheos { get; set; }
        public DbSet<RegistroEmpaque> Empaques { get; set; }
        public DbSet<RangoClasificacion> RangosClasificacion { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed rangos de clasificacion
            modelBuilder.Entity<RangoClasificacion>().HasData(
                new RangoClasificacion { Id = 1, Nombre = "Premium", Descripcion = "Flores de alta calidad, sin defectos", PuntajeMin = 90, PuntajeMax = 100, Color = "#FFD700" },
                new RangoClasificacion { Id = 2, Nombre = "Estándar", Descripcion = "Flores de calidad media, defectos leves", PuntajeMin = 70, PuntajeMax = 89, Color = "#C0C0C0" },
                new RangoClasificacion { Id = 3, Nombre = "Descarte", Descripcion = "Flores con defectos visibles", PuntajeMin = 0, PuntajeMax = 69, Color = "#CD7F32" }
            );
        }
    }
}
