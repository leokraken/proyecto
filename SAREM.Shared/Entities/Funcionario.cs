using System.ComponentModel.DataAnnotations;

namespace SAREM.Shared.Entities
{
    public class Funcionario
    {
        [Key]
        public string FuncionarioID { get; set; }
        [Required]
        public long nombre { get; set; }
        
    }
}
