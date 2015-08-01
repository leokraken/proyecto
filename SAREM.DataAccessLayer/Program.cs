using SAREM.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SARM.DataAccessLayer
{
    class Program
    {
        static void Main(string[] args)
        {
            //SARMContext.createTenant("test");
            //string schemasquery = @"select distinct sys.schemas.name from sys.schemas";
            //@"alter table Prueba.Pacientes drop foreign key Paciente_agendadas";

            using(var db = new SAREMAdminContext())
            {
                db.dropSchema("test");
            }
            /*
            var bd = SARMContext.getTenant("test");
            var iagenda = new DALAgenda("test");

            string CI = "50548305";
            //listar consultas
            foreach (var c in bd.consultas)
            {
                Debug.WriteLine(c.ConsultaID + " " + c.fecha_inicio.ToString());
            }
            foreach (var p in bd.pacientes)
            {
                Debug.WriteLine(p.PacienteID + " " + p.nombre);
            }

            //selecciono la primera consulta
            var consultas = from c in bd.consultas
                            select c;
            iagenda.agregarConsultaPaciente(CI, consultas.First().ConsultaID);

            */


            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
    }
}
