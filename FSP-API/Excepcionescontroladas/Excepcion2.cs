namespace FSP_API.Excepcionescontroladas
{
    public class Excepcion: ApplicationException
    {
        public Excepcion() : base("Usuario registrado en la base de datos") { }

    }
}
