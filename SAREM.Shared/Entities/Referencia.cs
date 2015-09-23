using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Referencia
    {
        public string PacienteID { get; set; }
        public string FuncionarioID { get; set; }
        [ForeignKey("PacienteID")]
        public virtual Paciente paciente { get; set; }
        [ForeignKey("FuncionarioID")]
        public virtual Medico medico { get; set; }

        public bool pendiente { get; set; }
        public DateTime fecha_solicitud { get; set; }
        public DateTime? fecha_confirmacion { get; set; }

    }
}
