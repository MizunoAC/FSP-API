namespace Proyecto_faunasilvestre.Modelos
{
    public class AnimalesCatalogo
    {
        public int AnimalesCatalogoId { get; set; }
        public string CEspecie { get; set; }

        public string CNombreComun { get; set; }
        public string CDescripcionAnimal { get; set; }

        public string CHabitat { get; set; }
        public string CHabitos { get; set; }
        public string CReproduccion { get; set; }

        public string CDistribucion { get; set; }
        public string CAlimentacion { get; set; }
        public byte[] DistribucionMapa { get; set; }    
        public byte[] CImagenAnimal { get; set; }
        public string CCategoria { get; set; }


        public virtual List<ModeloAnimales>? ModeloAnimales { get; set; }




    }
}
