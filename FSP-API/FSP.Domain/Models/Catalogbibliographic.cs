using FSP.Domain.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSP.Domain.Models
{

        public class Catalogbibliographic
    {
            //public string CommonNoun { get; set; }       // Nombre común (ej: "Tucán")
            //public string Specie { get; set; }           // Nombre científico
            //public string Category { get; set; }         // Categoría (ej: "Ave")
            //public string Description { get; set; }      // Descripción general
            //public string Habits { get; set; }           // Hábitos
            //public string Habitat { get; set; }          // Hábitat
            //public string Reproduction { get; set; }     // Reproducción
            //public string Feeding { get; set; }          // Alimentación
            //public string Distribution { get; set; }     // Distribución geográfica
            public CatalogDto Catalog { get; set; }
            //public List<CoordinateModel> Coordinates { get; set; } = new(); // Lista de ubicaciones
            public List<CatalogMapCords> Coordinates { get; set; } = new(); // Lista de ubicaciones
            public string Image { get; set; }            // Imagen en Base64 (opcional)
        }

        public class CoordinateModel
        {
            public double lat { get; set; }
            public double lng { get; set; }
        public string name { get; set; } = "Mexico";
        }
}
