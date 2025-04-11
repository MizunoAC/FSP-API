using System;

namespace FSP_API.Excepcionescontroladas
{
    public class Excepcion1 : ApplicationException
    {

        public Excepcion1() : base("Catalogo de Animales Inexistente") { }

        
    }
}
