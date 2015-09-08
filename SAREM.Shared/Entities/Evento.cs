using SAREM.Shared.enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Evento
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EventoID { get; set; }
        [Required]
        public string nombre { get; set; }
        public Sexo sexo { get; set; }

        public virtual ICollection<EventoPacienteComunicacion> pacientes { get; set; }
    }
}
