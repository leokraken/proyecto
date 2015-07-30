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
        [Required]
        public string mensaje { get; set; }
        [Required]
        public int dias { get; set; }

        public virtual ICollection<Paciente> pacientes { get; set; }
    }
}
