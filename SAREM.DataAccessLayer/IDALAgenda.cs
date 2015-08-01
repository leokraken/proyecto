using SAREM.Shared.Entities;
using System.Collections.Generic;

namespace SAREM.DataAccessLayer
{
    public interface IDALAgenda
    {
        void agregarConsultaPaciente(string PacienteID, long ConsultaID);
        void cancelarConsultaPaciente(string PacienteID, long ConsultaID);
        void ausenciaConsultaPaciente(string PacienteID, long ConsultaID);
        void agregarConsulta(Consulta consulta);
        void modificarConsulta(Consulta consulta);
        void eliminarConsulta(long ConsultaID);
        ICollection<Consulta> listarConsultas();
        ICollection<Consulta> listarConsultasPaciente(string PacienteID);
        ICollection<Consulta> listarConsultasCanceladasPaciente(string PacienteID);
        ICollection<Consulta> listarConsultasAusentesPaciente(string PacienteID);
    }
}
