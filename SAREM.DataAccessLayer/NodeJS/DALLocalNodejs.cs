using SAREM.DataAccessLayer.utils;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer.NodeJS
{
    public class DALLocalNodejs: IDALLocales
    {
        private string tenant;
        private Deserializer deserializer;

        public DALLocalNodejs(string tenant)
        {
            this.tenant = tenant;
            deserializer = new Deserializer(tenant);
        }
        public ICollection<Local> listarLocales()
        {
            return deserializer.get<List<Local>>("/locales/");
        }

        public Local obtenerLocal(long LocalID)
        {
            return deserializer.get<Local>("/locales/" + LocalID + "/");
        }

        public ICollection<Local> listarLocales(long EspecialidadID)
        {
            return deserializer.get<List<Local>>("/locales/especialidad/" + EspecialidadID + "/");
        }

        public ICollection<Local> listarLocalesMedico(string medicoID)
        {
            return deserializer.get<List<Local>>("/locales/medico/" + medicoID + "/");
        }
    }
}
