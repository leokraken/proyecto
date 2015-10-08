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
            f.adminController.dropSchema("NIST2010");
            f.adminController.createSchema("NIST2010");
            var list = f.adminController.getSchemas();
            foreach (var s in list)
            {
                Console.WriteLine(s);
            }
            Console.Read();
            //testOpenempi();
        }
    }
}
