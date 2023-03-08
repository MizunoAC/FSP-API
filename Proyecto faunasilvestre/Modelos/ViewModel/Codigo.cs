namespace Proyecto_faunasilvestre.Modelos.ViewModel
{
    public class Codigo
    { 
    

        public int Id { get; set; }
        public string Token { get; set; }
        public int ModeloUsuarioId { get; set; }
        public DateTime fecha { get; set; }
        public bool Usado { get; set; }
        public string Correo { get; set; }
        public virtual ModeloUsuario ModeloUsuario { get; set; }

    }
}
