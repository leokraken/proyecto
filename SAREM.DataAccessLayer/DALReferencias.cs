using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public class DALReferencias : IDALReferencias
    {
        private string tenant;
        public DALReferencias(string tenant)
        {
            this.tenant = tenant;
        }

        public ICollection<Paciente> obtenerPacientesReferenciadosMedico(string medicoID)
        {
            using(var db = SARMContext.getTenant(tenant)){
                var query = from r in db.referencias.Include("paciente")
                            where r.FuncionarioID == medicoID && !r.pendiente
                            select r.paciente;
                return query.ToList();
            }
        }
        public ICollection<Referencia> obtenerReferenciasPendientesMedico(string medicoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from r in db.referencias.Include("paciente")
                            where r.FuncionarioID == medicoID && r.pendiente
                            select r;
                return query.ToList();
            }
        }

        public ICollection<Referencia> obtenerTodasReferencias()
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                var query = from r in db.referencias.Include("medico").Include("paciente")
                            select r;
                return query.ToList();
            }            
        }

        public void agregarReferencia(string PacienteID, string MedicoID)
        {
            using(var db = SARMContext.getTenant(tenant))
            {
                Paciente paciente = (from p in db.pacientes.Include("medico")
                            where p.medico ==null && p.PacienteID==PacienteID
                            select p).Single();
                if (paciente.medico != null)
                {
                    throw new Exception("El paciente ya tiene un medico referenciado");
                }
                Funcionario medico = db.funcionarios.Find(MedicoID);

                if (paciente != null && medico != null)
                {
                    Referencia referencia = new Referencia { fecha_solicitud = DateTime.UtcNow, PacienteID=PacienteID, pendiente=true, FuncionarioID=MedicoID };
                    db.referencias.Add(referencia);
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("Medico a referenciar o paciente no existen.");
                }
            }
        }

        public void finalizarReferencia(string PacienteID, string MedicoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Referencia referencia = (from r in db.referencias
                            where r.PacienteID == PacienteID && r.FuncionarioID == MedicoID
                            select r).FirstOrDefault();
                if (referencia == null)
                {
                    throw new Exception("No existe tramite de referencia");
                }
                referencia.pendiente = false;
                referencia.fecha_confirmacion = DateTime.UtcNow;
                db.SaveChanges();
            }
        }

        public void denegarReferencia(string PacienteID, string MedicoID)
        {
            using (var db = SARMContext.getTenant(tenant))
            {
                Referencia r = (from re in db.referencias
                                where re.FuncionarioID == MedicoID && re.PacienteID == PacienteID
                                select re).Single();
                db.referencias.Remove(r);
                db.SaveChanges();
            }

        }


    }
}
