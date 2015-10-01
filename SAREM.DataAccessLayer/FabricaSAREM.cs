using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class FabricaSAREM
    {
        public IDALAgenda iagenda { get; set; }
        public IDALNotificaciones inotificaciones { get; set; }
        public IDALPacientes ipacientes { get; set; }
        public IDALReferencias ireferencias { get; set; }
        public IDALLocales ilocales { get; set; }
        public IDALMedicos imedicos { get; set; }
        public IDALEspecialidades iespecialidades { get; set; }
        public IDALOpenEMPI iopenempi { get; set; }

        public FabricaSAREM(string tenant)
        {
            iagenda = new DALAgenda(tenant);
            inotificaciones = new DALNotificaciones(tenant);
            ipacientes = new DALPacientes(tenant);
            ireferencias = new DALReferencias(tenant);
            ilocales = new DALLocales(tenant);
            imedicos = new DALMedicos(tenant);
            iespecialidades = new DALEspecialidades(tenant);
            //iopenempi = new DALOpenEMPI();
        }
    }
}
