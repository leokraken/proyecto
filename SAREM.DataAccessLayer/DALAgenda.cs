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
            Paciente paciente = db.pacientes.Find(PacienteID);
            if (paciente!=null)
            {
                Consulta consulta = db.consultas.Find(ConsultaID);
                if (consulta != null)
                {
                    PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID = ConsultaID, PacienteID = PacienteID, fecharegistro = DateTime.UtcNow };
                    db.consultasagendadas.Add(pca);
                    db.SaveChanges();
                }
                else
                    throw new Exception("No existe consulta");
                
            }
            else
            {
                throw new Exception("No existe paciente");
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
            var paciente = db.pacientes.Find(PacienteID);
            if (paciente!=null)
            {
                var consulta = db.consultas.Find(ConsultaID);
                if (consulta != null)
                {
                    consulta.ausencia = paciente;
                    db.SaveChanges();
                }
            }
            else
            {
                throw new Exception("No existe paciente");
            }
        }

        //Precondiciones: No tiene.
        public void agregarConsulta(Consulta consulta)
        {
            db.consultas.Add(consulta);            
        }

        public void modificarConsulta(Consulta consulta)
        {
            if (!db.consultas.Any(c => c.ConsultaID == consulta.ConsultaID))
                throw new Exception("No existe consulta");
            else
            {
                var c = db.consultas.Find(consulta.ConsultaID);
                c.EspecialidadID = consulta.EspecialidadID;
                c.fecha_fin = consulta.fecha_fin;
                c.fecha_inicio = consulta.fecha_inicio;
                c.FuncionarioID = consulta.FuncionarioID;
                db.SaveChanges();
            }
        }

        public void eliminarConsulta(long ConsultaID)
        {
            if(!db.consultas.Any(c => c.ConsultaID==ConsultaID))
                throw new Exception("No existe consulta");
            else
            {
                var c = db.consultas.Find(ConsultaID);
                db.consultas.Remove(c);
                db.SaveChanges();
            }
        }

        public ICollection<Consulta> listarConsultas()
        {
            //Entidades sin objetos dependencia 'datatypes'
            var query = from c in db.consultas
                        select c;
            return query.ToList();
        }

        public ICollection<Consulta> listarConsultasPaciente(string PacienteID)
        {
            var p = db.pacientes.Find(PacienteID);
            if (p == null)
                throw new Exception("No existe paciente");
            else
            {
                var q = from c in db.consultasagendadas.Include("consulta")
                        where c.PacienteID == PacienteID
                        select c.consulta;
                return q.ToList();
            }
        }

        public ICollection<Consulta> listarConsultasCanceladasPaciente(string PacienteID)
        {
            if (!db.pacientes.Any(p => p.PacienteID == PacienteID))
                throw new Exception("No existe paciente");

            var q = from c in db.consultascanceladas.Include("consulta")
                    where c.PacienteID == PacienteID
                    select c.consulta;
            return q.ToList();
        }

        public ICollection<Consulta> listarConsultasAusentesPaciente(string PacienteID)
        {
            var q = from c in db.pacientes.Include("ausencias")
                    where c.PacienteID == PacienteID
                    select c;
            return q.Single().ausencias;
        }

    }
}
