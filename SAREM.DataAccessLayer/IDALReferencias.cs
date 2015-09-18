﻿using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAREM.DataAccessLayer
{
    public interface IDALReferencias
    {
        ICollection<Paciente> obtenerPacientesReferenciadosMedico(string medicoID);
        ICollection<Paciente> obtenerReferenciasPendientesMedico(string medicoID);
        ICollection<Referencia> obtenerTodasReferencias();

        void agregarReferencia(string PacienteID, string MedicoID);
        void finalizarReferencia(string PacienteID, string MedicoID);
    }
}
