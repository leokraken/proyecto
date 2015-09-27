using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALEspecialidades : IDALEspecialidades
    {
        private string tenant;

        public DALEspecialidades(string tenant)
        {
            this.tenant = tenant;
        }

        public ICollection<Especialidad> listarEspecialidades()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                return db.especialidades.ToList();
            }
        }

        public ICollection<Especialidad> listarEspecialidadesLocal(long LocalID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var q = from e in db.locales.Include("especialidades")
                        where e.LocalID == LocalID
                        select e;
                var local = q.First();
                if (local != null)
                {
                    return local.especialidades.ToList();
                }
                else
                {
                    throw new Exception("No existe especialidad");
                }
            }
        }

        public Especialidad obtenerEspecialidad(long EspecialidadID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from e in db.especialidades
                            where e.EspecialidadID == EspecialidadID
                            select e;

                Especialidad esp = query.Single();
                return esp;
            }
        }

    }
}
