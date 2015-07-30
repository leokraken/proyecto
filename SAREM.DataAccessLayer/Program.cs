using SAREM.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARM.DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            //SARMContext.createTenant("Prueba");
            //string schemasquery = @"select distinct sys.schemas.name from sys.schemas";
            //@"alter table Prueba.Pacientes drop foreign key Paciente_agendadas";

            using(var db = new SAREMAdminContext())
            {
            }
            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
