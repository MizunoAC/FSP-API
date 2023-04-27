namespace Proyecto_faunasilvestre.Excepcionescontroladas
{
    public class Excepcion: ApplicationException
    {
        public Excepcion() : base("Usuario registrado en la base de datos") { }

    }
}
