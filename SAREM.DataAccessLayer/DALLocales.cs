using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALLocales : IDALLocales
    {
        private string tenant;
        
        public DALLocales(string tenant)
        {
            this.tenant = tenant;
        }

        public ICollection<Local> listarLocales()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.locales.ToList();
            }
        }

        public Local obtenerLocal(long LocalID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from l in db.locales
                            where l.LocalID == LocalID
                            select l;

                Local loc = query.Single();
                return loc;
            }
        }

    }
}
