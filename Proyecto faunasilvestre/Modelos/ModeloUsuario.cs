using Microsoft.EntityFrameworkCore;
using Proyecto_faunasilvestre.Modelos.TablaTemporal;
using Proyecto_faunasilvestre.Modelos.ViewModel;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_faunasilvestre.Modelos
{
    public class ModeloUsuario
    {

        
        public int ModeloUsuarioId { get; set; }

        [Required]
        [StringLength(20)]
        public string NombreUsuario { get; set; }
        [Required]
        [StringLength(20)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(20)]
        public string ApellidoPaterno { get; set; }

        public string ApellidoMaterno { get; set; }

        [Required]
        public string Localidad { get; set; }


        [StringLength(30)]
        public string Otralocalidad { get; set; }

        [Required]
        public string Sexo { get; set; }

        public int Edad { get; set; }

        [Required
        ,StringLength(50), 
         EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Contraseña { get; set; }


        public string TipoUsuario { get; set; }

        public virtual List<ModeloAnimales>? AnimalModels { get; set; }
        public virtual List<Codigo>? Tokens { get; set; }

        public virtual List<ModeloAnimalTemporal>? Temp { get; set; }

    }
}
