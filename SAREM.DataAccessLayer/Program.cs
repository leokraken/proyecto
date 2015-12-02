using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SAREM.DataAccessLayer;
using SAREM.DataAccessLayer.NodeJS;
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

            DataPaciente p = f.iopenempi.obtenerPaciente("UY", "50548305");
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

        public static void testEspecialidadControllerNodeJS()
        {
            string tenant = "test";
            Console.WriteLine("<---------TEST ESPECIALIDAD::-------->");

            DALEspecialidadNodejs dalesp = new DALEspecialidadNodejs(tenant);
            Especialidad e = dalesp.obtenerEspecialidad(1);
            Console.WriteLine(e.descripcion);
            Console.WriteLine("DALESPECIALIDAD::obtenerEspecialidad::OK");

            ICollection<Especialidad> lista_esp1 = dalesp.listarEspecialidades();
            foreach (var it in lista_esp1)
            {
                Console.WriteLine(it.descripcion);
            }
            Console.WriteLine("DALESPECIALIDAD::obtenerEspecialidades::OK");

            ICollection<Especialidad> lista_esp2 = dalesp.listarEspecialidadesLocal(1);
            foreach (var it in lista_esp2)
            {
                Console.WriteLine(it.descripcion);
            }
            Console.WriteLine("DALESPECIALIDAD::obtenerEspecialidadesLocal::OK");
        }

        public static void testLocalesControllerNodeJS()
        {
            string tenant = "test";
            Console.WriteLine("<---------TEST LOCAL::--------->");
            DALLocalNodejs dallocal = new DALLocalNodejs(tenant);

            Local local = dallocal.obtenerLocal(1);
            Console.WriteLine(local.calle);
            Local local_fail = dallocal.obtenerLocal(8);
            if (local_fail == null)
                Console.WriteLine("OK::LocalNULL");

            Console.WriteLine("DALLocales::obtenerLocal::OK");

            ICollection<Local> lista_1 = dallocal.listarLocales();
            foreach (var l in lista_1)
            {
                Console.WriteLine(l.calle);
            }

            Console.WriteLine("DALLocales::listarLocales::OK");

            ICollection<Local> lista_2 = dallocal.listarLocales(1);
            foreach (var l in lista_2)
            {
                Console.WriteLine(l.calle);
            }
            Console.WriteLine("DALLocales::listarLocales(EspecialidadID)::OK");

            ICollection<Local> lista_3 = dallocal.listarLocalesMedico("1234567");
            foreach (var l in lista_3)
            {
                Console.WriteLine(l.calle);
            }
            Console.WriteLine("DALLocales::listarLocalesMedico::OK");


        }

        public static void testReferenciaControllerNodeJS()
        {
            DALReferenciaNodejs refdal = new DALReferenciaNodejs("saremtest");
            ICollection<Referencia> list_1 = refdal.obtenerPacientesReferenciadosMedico("1234567");
            foreach (var r in list_1)
            {
                Console.WriteLine(r.PacienteID + "  " + r.FuncionarioID + " " + r.pendiente);
                Console.WriteLine(r.paciente.PaisID);
                Console.WriteLine(r.paciente.nombre);
            }
            Console.WriteLine("DALReferenciaNodeJS::obtenerPacientesReferenciadosMedico   OK");

            ICollection<Referencia> list_2 = refdal.obtenerReferenciasPendientesMedico("1234567");
            foreach (var r in list_2)
            {
                Console.WriteLine(r.PacienteID + "  " + r.FuncionarioID + " " + r.pendiente);
                Console.WriteLine(r.paciente.PaisID);
                Console.WriteLine(r.paciente.nombre);

            }
            Console.WriteLine("DALReferenciaNodeJS::obtenerReferenciasPendientesMedico   OK");

            ICollection<Referencia> list_3 = refdal.obtenerTodasReferencias();
            foreach (var r in list_3)
            {
                Console.WriteLine(r.PacienteID + "  " + r.FuncionarioID + " " + r.pendiente);
                Console.WriteLine(r.paciente.PaisID);
                Console.WriteLine(r.paciente.nombre);

            }
            Console.WriteLine("DALReferenciaNodeJS::obtenerTodasReferencias   OK");

            refdal.agregarReferencia("5434", "1234567");
            Console.WriteLine("DALReferenciaNodeJS::agregarReferencia   OK");

            Referencia r1 = refdal.obtenerReferencia("5434");
            Console.WriteLine(r1.PacienteID + "  " + r1.FuncionarioID + " " + r1.pendiente);
            Console.WriteLine("DALReferenciaNodeJS::obtenerReferencia   OK");

            refdal.denegarReferencia("5434", "1234567");
            Console.WriteLine("DALReferenciaNodeJS::denegarReferencia   OK");


            if(refdal.chequearExistenciaSolicitud("5432"))
                Console.WriteLine("DALReferenciaNodeJS::chequearExistenciaSolicitud   OK");
            else
                Console.WriteLine("DALReferenciaNodeJS::chequearExistenciaSolicitud   ERROR");

        }

        static void Main(string[] args)
        {

            //testLocalesControllerNodeJS();
            testReferenciaControllerNodeJS();
            Console.Read();             
        }
        /*
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
        */
        }
}
