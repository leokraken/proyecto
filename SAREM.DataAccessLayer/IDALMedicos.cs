using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALMedicos
    {
        ICollection<Medico> listarMedicosEspecialidadLocal(long LocalID, long EspecialidadID);
        Medico obtenerMedico(string FuncionarioID);
        ICollection<Medico> listarMedicosEspecialidad(long EspecialidadID);
        ICollection<Medico> listarMedicosLocal(long LocalID);
        ICollection<Medico> listarMedicos();
    }
}
