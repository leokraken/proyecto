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
        ICollection<Especialidad> listarEspecialidades();
        ICollection<Local> listarLocales();
        ICollection<Funcionario> listarFuncionarios();
        ICollection<Especialidad> listarEspecialidadesLocal(long LocalID);
        ICollection<Medico> listarMedicosEspecialidadLocal(long LocalID, long EspecialidadID);
        Consulta obtenerConsulta(long ConsultaID);
        Local obtenerLocal(long LocalID);
        Especialidad obtenerEspecialidad(long EspecialidadID);
        Medico obtenerMedico(string FuncionarioID);
    }
}
