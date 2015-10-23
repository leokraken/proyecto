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

        //TODO test

        public ICollection<Local> listarLocales(long EspecialidadID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = (from c in db.consultas.Include("locales")
                             where c.EspecialidadID == EspecialidadID
                             select c.local).Distinct();
                return query.ToList();

            }
        }

        public ICollection<Local> listarLocalesMedico(string medicoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = db.funcionarios.
                    Include("locales").
                    OfType<Medico>().
                    Where(m => m.FuncionarioID == medicoID);
                return query.First().locales.ToList();
                //return query.ToList();

            }
        }

    }
}
