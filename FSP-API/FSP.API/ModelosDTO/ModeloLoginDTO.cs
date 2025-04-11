using System.ComponentModel.DataAnnotations;

namespace FSP_API.ModelosDTO
{
    public class ModeloLoginDTO
    {

        [Required(ErrorMessage = "El campo Nombre de ususario es obligatorio")]
        [StringLength(20, ErrorMessage = "Debe contener menos de 20 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;

        public string Contraseña { get; set; } = string.Empty;

    }
}
