using System.ComponentModel.DataAnnotations;

namespace Proyecto_faunasilvestre.Modelos.TablaTemporal
{
    public class ModeloAnimalTemporal
    {

        public int Id { get; set; }

        public Boolean Aceptado { get; set; }
        public Boolean rechazado { get; set; }
        public int ModeloUsuarioId { get; set; }

        public int AnimalesCatalogoId { get; set; }

        public string NombreComun { get; set; }


        [Required]
        [StringLength(20)]
        public string CondicionAnimal { get; set; }

        [StringLength(200)]
        public string Descripcionanimal { get; set; }

        public string ImagenAnimal { get; set; }

        public string Ubicacion { get; set; }

        //public virtual List<AnimalesCatalogo>? AnimalesCatalogos { get; set; }

    }
}
