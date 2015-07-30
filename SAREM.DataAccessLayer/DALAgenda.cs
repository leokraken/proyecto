using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SAREM.DataAccessLayer
{
    public class DALAgenda: IDALAgenda
    {
        private SARMContext db = null;

        public DALAgenda(string tenant)
        {
            db = SARMContext.getTenant(tenant);
        }

        public void agregarConsultaPaciente(string PacienteID, long ConsultaID)
        {
            //check if exists
            if (db.consultas.Any(c => c.ConsultaID == ConsultaID) 
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
            {
                var agenda = new PacienteConsultaAgenda { ConsultaID=ConsultaID, PacienteID=PacienteID, fecharegistro= DateTime.UtcNow };
                db.consultasagendadas.Add(agenda);
                db.SaveChanges();
            }
            else
            {
                throw new Exception("No existe consulta o paciente");
            }
        }
        public void cancelarConsultaPaciente(string PacienteID, long ConsultaID)
        {
            if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
            {
                var cancelar = new PacienteConsultaCancelar {
                    ConsultaID = ConsultaID,
                    PacienteID = PacienteID,
                    fecha = DateTime.UtcNow 
                };
                db.consultascanceladas.Add(cancelar);
                db.SaveChanges();
            }
            else
            {
                throw new Exception("No existe consulta o paciente");
            }
        }

        public void ausenciaConsultaPaciente(string PacienteID, long ConsultaID)
        {
            if (db.consultas.Any(c => c.ConsultaID == ConsultaID)
                && db.pacientes.Any(p => p.PacienteID == PacienteID))
            {
                var ausencia = new PacienteConsultaAusencia
                {
                    ConsultaID = ConsultaID,
                    PacienteID = PacienteID,
                    fecha = DateTime.UtcNow
                };
                db.consultasausentes.Add(ausencia);
                db.SaveChanges();
            }
            else
            {
                throw new Exception("No existe consulta o paciente");
            }
        }
        public void agregarConsulta(Consulta consulta)
        {
            
        }
        public void modificarConsulta(Consulta consulta)
        {

        }
        public void eliminarConsulta(long ConsultaID)
        {

        }
        public ICollection<Consulta> listarConsultas()
        {
            throw new NotImplementedException();
        }
        public ICollection<Consulta> listarConsultasPaciente(string PacienteID)
        {
            throw new NotImplementedException();
        }
    }
}
