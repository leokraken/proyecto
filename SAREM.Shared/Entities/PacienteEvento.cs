
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SAREM.Shared.Entities
{
    public class PacienteEvento
    {
        public string PacienteID { get; set; }
        public long EventoID { get; set; }
        public long ComunicacionID { get; set; }

        [ForeignKey("PacienteID")]
        public virtual Paciente paciente { get; set; }
        [ForeignKey("EventoID")]
        public virtual EventoEstatico evento { get; set; }
        [ForeignKey("ComunicacionID")]
        public virtual Comunicacion comunicacion { get; set; }

    }
}
