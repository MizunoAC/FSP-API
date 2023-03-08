using System.ComponentModel.DataAnnotations;

namespace Proyecto_faunasilvestre.ModelosDTO
{
    public class ModeloUsuarioDTO
    {


        [Required(ErrorMessage = "El campo Nombre de ususario es obligatorio")]
        [StringLength(20, ErrorMessage = "Debe contener menos de 20 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Campo Nombre es obligatorio")]
        [StringLength(20, ErrorMessage = "Debe contener menos de 20 caracteres")]
        public string Nombre { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Campo Apellido Paterno es Obligatorio")]
        [StringLength(20, ErrorMessage = "Debe contener menos de 20 caracteres")]
        public string ApellidoPaterno { get; set; } = string.Empty;


        public string ApellidoMaterno { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Campo Localidad es Obligatorio")]
        public string Localidad { get; set; }


        [StringLength(30,ErrorMessage = "Debe contener menos de 30 Caracteres")]
        public string Otralocalidad { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Campo Sexo es Obligatorio")]
        public string Sexo { get; set; } = string.Empty;


        [Required(ErrorMessage = "El Campo Sexo es Obligatorio")]
        [Range (18,65, ErrorMessage = "La edad debe estar entre 18 y 65 años") ]
        public int Edad { get; set; }


        [Required(ErrorMessage = "El Campo Email es obligatorio"), StringLength(50), 
        EmailAddress(ErrorMessage = "Debe Ser un Correo Valido")] 
        public string Email { get; set; } = string.Empty; 


        [Required]
        [StringLength(10, ErrorMessage = "Debe contener Maximo 10 caracteres")]
        public string Contraseña { get; set; } = string.Empty;

        public string ConfirmarContraseña { get; set; } = string.Empty;


        public string TipoUsuario = "Usuario";


    }
}
