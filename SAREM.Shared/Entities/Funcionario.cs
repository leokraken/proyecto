using System.ComponentModel.DataAnnotations;

namespace SAREM.Shared.Entities
{
    public abstract class Funcionario
    {
        [Key]
        public string FuncionarioID { get; set; }
        [Required]
        public string nombre { get; set; }
        
    }
}
