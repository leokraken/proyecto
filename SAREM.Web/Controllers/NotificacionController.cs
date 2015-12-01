using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using SAREM.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SAREM.Web.Controllers
{
   

    public class NotificacionController : Controller
    {
        private FabricaSAREM fabrica;
        private string paciente;
        private string tenant;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["usuario"] != null)
            {
                Debug.WriteLine("USUARIO NO ES NULL...");
                paciente = (string)filterContext.HttpContext.Session["usuario"];
                tenant = (string)filterContext.HttpContext.Session["tenant"];
                fabrica = new FabricaSAREM(tenant);
            }
            else
            {
                Debug.WriteLine("USUARIO NULL...");
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { action = "GetLogOff", controller = "Account" }));
            }
        }



        #region DataTypes
        public class EventoJSON
        {
            public string EventoID { get; set; }
            public string nombre { get; set; }
            public string mensaje { get; set; }
            public string sexo { get; set; }
            //Op y Ob
            public string tipo { get; set; }
            public string fechaNot { get; set; }
            public List<string> edades { get; set; }
        }
        
        public class PacienteJSON
        {
            public string PacienteID { get; set; }
            public string nombre { get; set; }
            public string medio { get; set; }
           
        }

        #endregion

        public ActionResult Index()
        {
            return View("VerNotificaciones");
        }

        //public JsonResult GetEventosOb()
        //{
        //    //var eventos = notis.;
        //    List<EventoJSON> lista = new List<EventoJSON>();

        //    foreach (Evento e in eventos)
        //    {
        //        EventoJSON ejson = new EventoJSON();
        //        //ejson.dias = e.dias.ToString();
        //        ejson.EventoID = e.EventoID.ToString();
        //        //ejson.mensaje = e.mensaje;
        //        ejson.nombre = e.nombre;
        //        lista.Add(ejson);
        //    }

        //    return Json(lista, JsonRequestBehavior.AllowGet);
        //}


        
        public ActionResult VerEventos()
        {
            return View("VerNotificaciones");
        }

        // GET: Notificacion/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notificacion/Create
        [HttpPost]
        public JsonResult Create(EventoJSON e)
        {
            try
            {

                if (e.tipo.Equals("Ob"))
                {

                    SAREM.Shared.Entities.EventoObligatorio eOb = new SAREM.Shared.Entities.EventoObligatorio();
                    eOb.nombre = e.nombre;
                    if (e.sexo.Equals("A"))
                    {
                        eOb.sexo = Shared.enums.Sexo.AMBOS.ToString();

                    }
                    else if (e.sexo.Equals("F"))
                    {
                        eOb.sexo = Shared.enums.Sexo.FEMENINO.ToString();
                    }
                    else
                    {
                        eOb.sexo = Shared.enums.Sexo.MASCULINO.ToString();
                    }

                    eOb.mensaje = e.mensaje;
                    eOb.fechanotificacion = ConsultaController.ParseDate(e.fechaNot).ToUniversalTime();

                    fabrica.inotificaciones.crearEvento(eOb);
                }
                else if (e.tipo.Equals("Op"))
                {
                    SAREM.Shared.Entities.EventoOpcional eOp = new SAREM.Shared.Entities.EventoOpcional();
                    eOp.nombre = e.nombre;
                    if (e.sexo.Equals("A"))
                    {
                        eOp.sexo = Shared.enums.Sexo.AMBOS.ToString();

                    }
                    else if (e.sexo.Equals("F"))
                    {
                        eOp.sexo = Shared.enums.Sexo.FEMENINO.ToString();
                    }
                    else
                    {
                        eOp.sexo = Shared.enums.Sexo.MASCULINO.ToString();
                    }

                    eOp.mensaje = e.mensaje;
                    eOp.edadesarray = string.Join(",", e.edades.ToArray()); ;
                    fabrica.inotificaciones.crearEvento(eOp);
                }

              
                return Json(new { success = true });

            }
            catch
            {
                return Json(new { success = false });
            }
           

        }

        public JsonResult GetEventosObligatorios()
        {
            var eventosOb = fabrica.inotificaciones.listarEventosObligatorios();
            List<EventoJSON> lista = new List<EventoJSON>();
            foreach (EventoObligatorio e in eventosOb)
            {
                EventoJSON ejs = new EventoJSON();
                ejs.nombre = e.nombre;
                ejs.sexo = e.sexo.ToString();
                ejs.mensaje = e.mensaje;
                String format = "dd/MM/yyyy HH:mm";
                DateTime runtimeKnowsThisIsUtc = DateTime.SpecifyKind(
                        e.fechanotificacion,
                            DateTimeKind.Utc);
                DateTime localVersionFIni = runtimeKnowsThisIsUtc.ToLocalTime();
                ejs.fechaNot = localVersionFIni.ToString(format);
                ejs.EventoID = e.EventoID.ToString();
                lista.Add(ejs);

            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEventosOpcionales()
        {
            var eventosOp = fabrica.inotificaciones.listarEventosOpcionales();
            List<EventoJSON> lista = new List<EventoJSON>();
            foreach (EventoOpcional e in eventosOp)
            {
                EventoJSON ejs = new EventoJSON();
                ejs.nombre = e.nombre;
                ejs.sexo = e.sexo.ToString();
                ejs.mensaje = e.mensaje;
             
               
                ejs.EventoID = e.EventoID.ToString();
                lista.Add(ejs);

            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEdades(string idE)
        {
            try
            {
                var lista = fabrica.inotificaciones.getEdadesEvento(Convert.ToInt64(idE));
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { success = false });
            }
           
        }

        public JsonResult GetPacientesEvento(string idE)
        {
            try
            {
                var listaPacientes = fabrica.inotificaciones.listarEventosPaciente(Convert.ToInt64(idE));
                List<PacienteJSON> lista = new List<PacienteJSON>();
                foreach (EventoPacienteComunicacion e in listaPacientes)
                {
                    PacienteJSON pj = new PacienteJSON();
                    pj.PacienteID = e.paciente.PacienteID;
                    pj.nombre = e.paciente.nombre;
                    pj.medio = e.comunicacion.nombre;

                    lista.Add(pj);
                }
                return Json(lista, JsonRequestBehavior.AllowGet);
            }
            catch(Exception e)
            {
                return Json(new { success = false });
            }

        }

        [HttpGet]
        public ActionResult VerPacientesEvento(string idE)
        {
           
            var e = fabrica.inotificaciones.obtenerEvento(Convert.ToInt64(idE));

            EventoWeb ew = new EventoWeb();
            ew.nombre = e.nombre;
            ew.sexo = e.sexo.ToString();
            ew.msj = e.mensaje;
            ew.EventoID = e.EventoID.ToString();

            if (e is EventoObligatorio)
            {
                ew.tipo = "OBLIGATORIO";
            }
            else
            {
                ew.tipo = "OPCIONAL";
            }


            return View("VerEventosPaciente", ew);
        }

        [HttpPost]
        public JsonResult Delete(string idE)
        {
            try
            {
                fabrica.inotificaciones.eliminarEvento(Convert.ToInt64(idE));
                return Json(new { success = true });

            }
            catch
            {
                return Json(new { success = false });
            }


        }

        [HttpGet]
        public JsonResult GetPacientesEventoNew(string idE)
        {
            long idL = Convert.ToInt64(idE);
            var pacientesNotInConsulta = fabrica.inotificaciones.listarPacientesNotInEvento(idL);
            var pacientes =
                    from p in pacientesNotInConsulta
                    select new { PacienteID = p.PacienteID };

            return Json(pacientes, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddPacienteEvento(string idE, string idP)
        {
            try
            {
                fabrica.inotificaciones.suscribirPacienteEvento(Convert.ToInt64(idE), idP, 3);
               
                return Json(new { success = true });

            }
            catch
            {
                return Json(new { success = false });
            }


        }
    }
}
