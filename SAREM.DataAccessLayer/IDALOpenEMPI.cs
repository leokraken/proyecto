using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALOpenEMPI
    {
        person obtenerPaciente(string paisID, string pacienteID);
        List<identifierDomain> obtenerDominios();
    }
}
