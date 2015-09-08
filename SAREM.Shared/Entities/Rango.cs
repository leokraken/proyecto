using SAREM.Shared.enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared.Entities
{
    public class Rango
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RangoID { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required, Range(18, 110)]
        public short limitei { get; set; }
        [Required, Range(18, 110)]
        public short limites { get; set; }
        
        public virtual ICollection<EventoOpcional> eventos { get; set; }
    }
}
