using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALPaises : IDALPaises
    {
        public ICollection<Pais> obtenerPaises()
        {
            List<Pais> lista = new List<Pais>
            {
                new Pais{PaisID="UY", nombre="Uruguay"},
                new Pais{PaisID="BR", nombre="Brasil"}
            };
            return lista;
        }
    }
}
