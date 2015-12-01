using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Local
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LocalID { get; set; }
        [Required]
        public string calle { get; set; }
        [Required]
        public string numero { get; set; }
        public string nombre { get; set; }
        public Double? latitud { get; set; }
        public Double? longitud { get; set; }

        public virtual ICollection<Consulta> consultas { get; set; }
        public virtual ICollection<Especialidad> especialidades { get; set; }
        public virtual ICollection<Medico> medicos { get; set; }
    }
}
