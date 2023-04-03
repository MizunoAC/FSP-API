using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_faunasilvestre.Modelos
{
    public class ModeloAnimales
    {


        
        public int ModeloAnimalesId { get; set; }


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