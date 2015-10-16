using SAREM.Shared.Datatypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALOpenEMPI
    {
        DataPaciente obtenerPaciente(string paisID, string pacienteID);
        DataPaciente obtenerPacienteParallel(string paisID, string pacienteID);
        void getAuth();

        //admin
        List<identifierDomain> obtenerDominios();
        void agregarDominio(identifierDomain dominio);

    }
}
