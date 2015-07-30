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
    }
}
