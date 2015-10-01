using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;

namespace SAREM.DataAccessLayer
{
    public interface IDALAgenda
    {
        void agregarConsultaPaciente(string PacienteID, long ConsultaID, Boolean fueraLista);
        //TODO test
        DateTime ? agregarConsultaPaciente(string PacienteID, long ConsultaID);

        void cancelarConsultaPaciente(string PacienteID, long ConsultaID);
        void ausenciaConsultaPaciente(string PacienteID, long ConsultaID);
        void agregarConsulta(Consulta consulta);
        void modificarConsulta(Consulta consulta);
        void eliminarConsulta(long ConsultaID);
        ICollection<Consulta> listarConsultas();
        ICollection<Consulta> listarConsultasPaciente(string PacienteID);
        ICollection<Consulta> listarConsultasCanceladasPaciente(string PacienteID);
        ICollection<Consulta> listarConsultasAusentesPaciente(string PacienteID);
        ICollection<Consulta> listarConsultasMedicoLocalEspecialidad(long EspecialidadID, long LocalID, string MedicoID, DateTime fecha);


        ICollection<Funcionario> listarFuncionarios();
        Consulta obtenerConsulta(long ConsultaID);
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

        //obtener parametros consulta, max pacientes consulta y max pacientes lista de espera
        DataParametros obtenerParametrosConsulta();
        //Obtengo consulta para buscar el turno
        PacienteConsultaAgenda obtenerConsulta(string PacienteID, long ConsultaID);
        //chequear si paciente pertenece a la consulta o a la lista de espera, true si pertenece a la consulta, false en caso contrario
        Boolean perteneceConsulta(string PacienteID, long ConsultaID);
    }
}
