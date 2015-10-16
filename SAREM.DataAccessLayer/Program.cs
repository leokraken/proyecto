using SAREM.DataAccessLayer;
using SAREM.Shared.Datatypes;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SARM.DataAccessLayer
{
    class Program
    {
        public static void testOpenempi()
        {
            FabricaSAREM f = new FabricaSAREM("test");
            Console.WriteLine("Openempi test.");

            DataPaciente p = f.iopenempi.obtenerPaciente("UY", "5432");
            if (p != null){
                Console.WriteLine(p.paciente.mail);
                Console.WriteLine(p.mutualista);
            }
            else
                Console.WriteLine("Persona no encontrada...");

            Console.WriteLine("Proceso finalizado...");
            Console.Read();
        }
        
        static void Main(string[] args)
        {
            FabricaSAREM f = new FabricaSAREM();

            //f.adminController.dropSchema("test");
            
            //f.adminController.createSchema("test");
            FabricaSAREM fab = new FabricaSAREM("test");
            foreach (var a in fab.iagenda.obtenerTurnosLibres(4))
            {
                Console.WriteLine(a.turno);
            }


            double minutos = (DateTime.UtcNow - DateTime.UtcNow.AddMinutes(300)).TotalMinutes;
            //fab.iagenda.agregarConsultaPaciente("1",1,0,false);
            /*
            var list = f.adminController.getSchemas();
            using (var db = SARMContext.getTenant("test"))
            {
                PacienteConsultaAgenda pca = new PacienteConsultaAgenda { ConsultaID=4, fueralista=false, fecharegistro=DateTime.Now, turno=DateTime.Now };
                db.consultasagendadas.Add(pca);
                db.SaveChanges();
            }
            foreach (var s in list)
            {
                Console.WriteLine(s);
            }
             * */
            Console.Read();
            //testOpenempi();
        }
    }
}
