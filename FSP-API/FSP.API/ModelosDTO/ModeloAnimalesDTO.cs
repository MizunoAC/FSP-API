using System.ComponentModel.DataAnnotations;

namespace FSP_API.ModelosDTO
{
    public class ModeloAnimalesDTO
    {
        public  int ModeloUsuarioId { get; set; }

        //public virtual int AnimalesCatalogoId { get; set; }

        public string NombreComun { get; set; }

        public string ImagenAnimal { get; set; }


        [Required(ErrorMessage = "El Campo Condicion Animal es Obligatorio")]
        [StringLength(20, ErrorMessage = "Debe contener menos de 20 caracteres")]
        public string CondicionAnimal { get; set; }

        [StringLength(200, ErrorMessage = "Debe contener menos de 200 caracteres")]
        public string Descripcionanimal { get; set; }

        public string Ubicacion { get; set; }

        public string? ImgBase64 { get; set; }

    }
}
