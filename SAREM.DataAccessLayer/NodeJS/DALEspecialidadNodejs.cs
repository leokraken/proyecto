using SAREM.DataAccessLayer.utils;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer.NodeJS
{
    public class DALEspecialidadNodejs : IDALEspecialidades
    {
        private string tenant;
        private Deserializer deserializer;
        public DALEspecialidadNodejs(string tenant)
        {
            this.tenant = tenant;
            deserializer = new Deserializer(tenant);
        }

        public Especialidad obtenerEspecialidad(long EspecialidadID)
        {
            return deserializer.get<Especialidad>("/especialidades/" + EspecialidadID + "/");
        }

        public ICollection<Especialidad> listarEspecialidades()
        {
            return deserializer.get<List<Especialidad>>("/especialidades/");
        }

        public ICollection<Especialidad> listarEspecialidadesLocal(long LocalID) 
        {
            return deserializer.get<List<Especialidad>>("/especialidades/"+LocalID+"/especialidades");
        }
    }
}
