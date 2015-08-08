using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Especialidad
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long EspecialidadID { get; set; }
        public string descripcion { get; set; }
        [Required]
        public string tipo { get; set; }

        public virtual ICollection<Medico> medicos { get; set; }
        public virtual ICollection<Consulta> consultas { get; set; }
        public virtual ICollection<Local> locales { get; set; }
    }
}
