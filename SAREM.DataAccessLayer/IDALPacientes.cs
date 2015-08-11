using SAREM.Shared.Entities;
using System.Collections.Generic;

namespace SAREM.DataAccessLayer
{
    public interface IDALPacientes
    {
        Paciente obtenerPaciente(string CI);
        void altaPaciente(Paciente paciente);
        void modificarPaciente(Paciente paciente);
        void sancionarPaciente(string CI);
        void eliminarPaciente(string CI);
        ICollection<Paciente> listarPacientes();
    }
}
