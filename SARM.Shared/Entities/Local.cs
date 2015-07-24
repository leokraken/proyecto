﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAREM.Shared
{
    public class Local
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LocalID { get; set; }
        [Required]
        public string calle { get; set; }
        [Required]
        public string numero { get; set; }
        
        public double latitud { get; set; }
        public double longitud { get; set; }

        public virtual ICollection<Consulta> consultas { get; set; }
    }
}
