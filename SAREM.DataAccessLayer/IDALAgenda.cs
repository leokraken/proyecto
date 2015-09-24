using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;

namespace SAREM.DataAccessLayer
{
    public interface IDALAgenda
    {
        void agregarConsultaPaciente(string PacienteID, long ConsultaID, Boolean fueraLista);
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
        ICollection<Funcionario> listarFuncionarios();
        ICollection<Especialidad> listarEspecialidadesLocal(long LocalID);
        ICollection<Medico> listarMedicosEspecialidadLocal(long LocalID, long EspecialidadID);
        Consulta obtenerConsulta(long ConsultaID);
        Especialidad obtenerEspecialidad(long EspecialidadID);
        Medico obtenerMedico(string FuncionarioID);
        void agregarConsultaPacienteEspera(string PacienteID, long ConsultaID);
        void eliminarPacienteConsultaLE(string PacienteID, long ConsultaID);
        void moverPacientesLEConsulta(List<string> pacientesIDs, long ConsultaID);
        ICollection<Paciente> listarPacientesNotInConsulta(long ConsultaID);
    
        ICollection<Paciente> obtenerPacientesConsulta(long ConsultaID);
        ICollection<Paciente> obtenerPacientesConsultaEspera(long ConsultaID);
        ICollection<Paciente> obtenerPacientesConsultaFueraLista(long ConsultaID);

        //parte diario
        ICollection<Consulta> obtenerParteDiario(string MedicoID, DateTime fecha);
        void actualizarParteDiario(long ConsultaID, string PacienteID, string diagnostico, bool ausencia);
        //Obtengo diagnostico y ausencia de paciente en consulta
        PacienteConsultaAgenda obtenerPacienteConsulta(long ConsultaID, string PacienteID);
    }
}
