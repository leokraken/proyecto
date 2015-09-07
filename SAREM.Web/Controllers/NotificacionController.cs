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
            public string dias { get; set; }
        }
        

        // GET: Notificacion
        public ActionResult Index()
        {
            return View("VerNotificaciones");
        }

        //Obtener todos los eventos/notificaciones del tenant
        public JsonResult GetEventos()
        {
            var eventos = notis.listarEventos();
            List<EventoJSON> lista = new List<EventoJSON>();

            foreach (Evento e in eventos)
            {
                EventoJSON ejson = new EventoJSON();
                ejson.dias = e.dias.ToString();
                ejson.EventoID = e.EventoID.ToString();
                ejson.mensaje = e.mensaje;
                ejson.nombre = e.nombre;
                lista.Add(ejson);
            }

            return Json(lista, JsonRequestBehavior.AllowGet);
        }


        // GET: Notificacion/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Notificacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notificacion/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
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
