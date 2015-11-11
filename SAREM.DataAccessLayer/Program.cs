using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
using System.Web.Script.Serialization;
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

        static void amq()
        {
            DALAMQP amq = new DALAMQP("test");
            var m = new DataMensaje
            {
                medio = 1, //mail
                destinatario = "chebaysend@gmail.com",
                asunto = "RabbitMQ testing",
                mensaje = "Testing python send mail!",
                fecha_envio = DateTime.UtcNow.AddMinutes(1),
                inmediato=false
            };
            amq.sendToQueue(m);
        }

        void testConsultamail() {
            FabricaSAREM f = new FabricaSAREM("test");
            /*
            Consulta c = new Consulta
            {
                EspecialidadID = 1,
                fecha_inicio = DateTime.UtcNow.AddMinutes(2),
                fecha_fin = DateTime.UtcNow.AddMinutes(12),
                FuncionarioID = "17299995",
                LocalID = 1,
                numpacientes = 10,
                maxpacientesespera = 10
            };
            f.iagenda.agregarConsulta(c);
            long consultaid = 
             * */
           
        }

        static T get<T>(string url)
        {
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(url);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(str);
                var o = JsonConvert.DeserializeObject<T>(str);
                return o;
            }
        }

        static void post<T>(string url, T o)
        {
            using (var client = new HttpClient())
            {
                var result = client.PostAsJsonAsync(url,o);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(str);
                //var o = JsonConvert.DeserializeObject<T>(str);
                //return o;
            }
        }

        static void Main(string[] args)
        {
            List<DataConsulta> lista = new List<DataConsulta>();
            FabricaSAREM f = new FabricaSAREM("test");
            //f.adminController.dropSchema("test");
            //f.adminController.createSchema("test");
                //new FabricaSAREM("test");
            //f.iagenda.cancelarConsultaPaciente("1", 2);
            //f.iagenda.agregarConsultaPaciente("0", 1, 1, false);
            //f.iagenda.agregarConsultaPaciente("50548305", 1, 1, false);
            //amq();
            //var q = f.iagenda.listarConsultasPaciente("0");
            //Console.WriteLine("paciente 0");
            //f.iagenda.agregarConsultaPacienteEspera("0", 3);
            var cons = f.iagenda.listarConsultasPaciente("0");
            foreach (var c in cons)
            {
                Console.WriteLine(c.consulta.ConsultaID+" "+c.consulta.EspecialidadID + " "+c.espera);
            }

            Console.Read();
            
            
            
            
            
            
            
            
            
            
            /*
            string url = @"http://10.0.2.2:3000/api/pacientes/";
            var o = get<List<Paciente>>(url);
            foreach (var e in o)
            {
                Console.WriteLine(e.PacienteID);
                Console.WriteLine(e.nombre);
            }
            Paciente p = new Paciente{ PacienteID="5432", sancion=false, sexo="M", nombre="Leonardin"};
            post<Paciente>(url, p);

            Console.WriteLine("finish");
            Console.Read();
             */
            //testOpenempi();
        }

        public static void clienthttp()
        {
            string url= @"http://10.0.2.2:3000/api/especialidades/";
            using (var client = new HttpClient())
            {
                var result = client.GetAsync(url);
                string str = result.Result.Content.ReadAsStringAsync().Result;
                Console.WriteLine(str);
                var o = JsonConvert.DeserializeObject<List<Especialidad>>(str);
                //JArray list = JArray.Parse(str);

                //JavaScriptSerializer ser = new JavaScriptSerializer();

                //var r = ser.Deserialize<List<Especialidad>>(str);    
                foreach (var e in o)
                {
                    Console.WriteLine(e.EspecialidadID);

                }
             
            }
        }

        }
}
