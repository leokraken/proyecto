using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Rango
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public short limitei { get; set; }
        [Required]
        public short limites { get; set; }
        public bool sexo { get; set; }

        public virtual ICollection<EventoEstatico> eventos { get; set; }
    }
}
