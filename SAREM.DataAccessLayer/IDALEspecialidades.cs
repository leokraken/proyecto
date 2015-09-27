using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALEspecialidades
    {
        Especialidad obtenerEspecialidad(long EspecialidadID);
        ICollection<Especialidad> listarEspecialidades();
        ICollection<Especialidad> listarEspecialidadesLocal(long LocalID);

    }
}
