using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALReferencias
    {
        ICollection<Referencia> obtenerPacientesReferenciadosMedico(string medicoID);
        ICollection<Referencia> obtenerReferenciasPendientesMedico(string medicoID);
        ICollection<Referencia> obtenerTodasReferencias();

        void agregarReferencia(string PacienteID, string MedicoID);
        void finalizarReferencia(string PacienteID, string MedicoID);
        void denegarReferencia(string PacienteID, string MedicoID);
        Referencia obtenerReferencia(string PacienteID);
        Boolean chequearExistenciaSolicitud(string PacienteID);
    }
}
