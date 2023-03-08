using System;

namespace Proyecto_faunasilvestre.Excepcionescontroladas
{
    public class Excepcion1 : ApplicationException
    {

        public Excepcion1() : base("Catalogo de Animales Inexistente") { }

        
    }
}
