using SAREM.DataAccessLayer.utils;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer.NodeJS
{
    public class DALReferenciaNodejs : IDALReferencias
    {
        private string tenant;
        private Deserializer deserializer;

        public DALReferenciaNodejs(string tenant)
        {
            this.tenant = tenant;
            deserializer = new Deserializer(tenant);
        }

        public ICollection<Referencia> obtenerPacientesReferenciadosMedico(string medicoID)
        {
            return deserializer.get<List<Referencia>>("/referencias/"+medicoID+"/");

        }

        public ICollection<Referencia> obtenerReferenciasPendientesMedico(string medicoID)
        {
            return deserializer.get<List<Referencia>>("/referencias/" + medicoID + "/pendientes");
        }

        public ICollection<Referencia> obtenerTodasReferencias()
        {
            return deserializer.get<List<Referencia>>("/referencias/");
        }

        public void agregarReferencia(string PacienteID, string MedicoID) {
            deserializer.post<Object>("/referencias/", new {FuncionarioID= MedicoID, PacienteID=PacienteID});
        }

        public void finalizarReferencia(string PacienteID, string MedicoID)
        {
            deserializer.put<Object>("/referencias/", new { FuncionarioID = MedicoID, PacienteID = PacienteID });
        }
        public void denegarReferencia(string PacienteID, string MedicoID)
        {
            deserializer.delete("/referencias/" + PacienteID);
        }

        public Referencia obtenerReferencia(string PacienteID)
        {
            return deserializer.get<Referencia>("/paciente/" + PacienteID + "/referencia");
        }

        public Boolean chequearExistenciaSolicitud(string PacienteID)
        {
            return (deserializer.get<Referencia>("/paciente/" + PacienteID + "/referencia")!=null);
        }
    }
}
