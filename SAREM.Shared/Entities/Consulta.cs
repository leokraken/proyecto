using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared
{
    public class Consulta
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ConsultaID { get; set; }

        [Required]
        public DateTime fecha_inicio { get; set; }
        [Required]
        public DateTime fecha_fin { get; set; }

        public virtual PacienteConsultaAgenda paciente {get; set;}
        public virtual Especialidad especialidad { get; set; }
        public virtual Medico medico { get; set; }

        public virtual ICollection<AgendaEvento> notificaciones { get; set; }
    }
}
