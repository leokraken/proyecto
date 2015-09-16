using SAREM.DataAccessLayer;
using SAREM.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAREM.Web.Controllers
{
    public class NotificacionController : Controller
    {
        IDALNotificaciones notis = new DALNotificaciones("test");

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
        

        // GET: Notificacion
        public ActionResult Index()
        {
            return View("VerNotificaciones");
        }

        //Obtener todos los eventos/notificaciones del tenant
        //public JsonResult GetEventos()
        //{
        //    //var eventos = notis.listarEventos();
        //    //List<EventoJSON> lista = new List<EventoJSON>();

        //    //foreach (Evento e in eventos)
        //    //{
        //    //    EventoJSON ejson = new EventoJSON();
        //    //    //ejson.dias = e.dias.ToString();
        //    //    ejson.EventoID = e.EventoID.ToString();
        //    //    //ejson.mensaje = e.mensaje;
        //    //    ejson.nombre = e.nombre;
        //    //    lista.Add(ejson);
        //    //}

        //    //return Json(lista, JsonRequestBehavior.AllowGet);
        //}


        // GET: Notificacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
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
                        eOb.sexo = Shared.enums.Sexo.AMBOS;

                    }
                    else if (e.sexo.Equals("F"))
                    {
                        eOb.sexo = Shared.enums.Sexo.FEMENINO;
                    }
                    else
                    {
                        eOb.sexo = Shared.enums.Sexo.MASCULINO;
                    }

                    eOb.mensaje = e.mensaje;
                    eOb.fechanotificacion = ConsultaController.ParseDate(e.fechaNot).ToUniversalTime();

                    notis.crearEvento(eOb);
                }
                else if (e.tipo.Equals("Op"))
                {
                    SAREM.Shared.Entities.EventoOpcional eOp = new SAREM.Shared.Entities.EventoOpcional();
                    eOp.nombre = e.nombre;
                    if (e.sexo.Equals("A"))
                    {
                        eOp.sexo = Shared.enums.Sexo.AMBOS;

                    }
                    else if (e.sexo.Equals("F"))
                    {
                        eOp.sexo = Shared.enums.Sexo.FEMENINO;
                    }
                    else
                    {
                        eOp.sexo = Shared.enums.Sexo.MASCULINO;
                    }

                    eOp.mensaje = e.mensaje;
                    eOp.edadesarray = string.Join(",", e.edades.ToArray()); ;
                    notis.crearEvento(eOp);
                }

              
                return Json(new { success = true });

            }
            catch
            {
                return Json(new { success = false });
            }
           

        }

        // GET: Notificacion/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Notificacion/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Notificacion/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Notificacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
