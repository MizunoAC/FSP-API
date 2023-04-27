using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Modelos;
using Proyecto_faunasilvestre.Modelos.TablaTemporal;
using Proyecto_faunasilvestre.Modelos.ViewModel;

namespace Proyecto_faunasilvestre.Context
{
    public class ContexDb : DbContext
    {


        protected readonly IConfiguration Configuration;

        public ContexDb(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(Configuration.GetConnectionString("DefaultCnt"));
        }

        public DbSet<ModeloUsuario> ModeloUsuarios { get; set; }
        public DbSet<ModeloAnimales> ModeloAnimales { get; set; }

        public DbSet<AnimalesCatalogo> AnimalesCatalogos { get; set; }
        public DbSet<Codigo> codigos { get; set; }
      
        public DbSet<ModeloAnimalTemporal> Temporal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder
                .Entity<ModeloAnimalTemporal>()
                .ToTable("Temporal", b => b.IsTemporal()); 
           
        }




    }
}
