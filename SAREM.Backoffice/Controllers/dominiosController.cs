using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SAREM.Backoffice.Models;
using SAREM.DataAccessLayer;
using System.Diagnostics;

namespace SAREM.Backoffice
{
    public class dominiosController : Controller
    {
        private SAREMAdminContext db = new SAREMAdminContext();
        private FabricaSAREM f = new FabricaSAREM();
        // GET: dominios
        public ActionResult Index()
        {
            ViewBag.dominios = f.adminController.getSchemas();
            return View(f.iopenempi.obtenerDominios());
        }

        [HttpGet]
        public ActionResult Agregar(string dominio)
        {
            Debug.WriteLine("agrega edl dominiooooooooo");
            Debug.WriteLine(dominio);
            try
            {
                f.adminController.createSchema(dominio);
            }
            catch (Exception e)
            {
                Debug.WriteLine("Ocurrio un error al agregar el dominio "+dominio);
            }

            return Json("{}", JsonRequestBehavior.AllowGet);
        }


        // GET: dominios/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            identifierDomain identifierDomain = f.iopenempi.obtenerDominios().Where(d => d.identifierDomainId==id).First();
            if (identifierDomain == null)
            {
                return HttpNotFound();
            }
            return View(identifierDomain);
        }

        // GET: dominios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: dominios/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "identifierDomainId,dateCreated,dateCreatedSpecified,identifierDomainDescription,identifierDomainIdSpecified,identifierDomainName,namespaceIdentifier,universalIdentifier,universalIdentifierTypeCode")] identifierDomain identifierDomain)
        {
            if (ModelState.IsValid)
            {
                //db.identifierDomains.Add(identifierDomain);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(identifierDomain);
        }

        // GET: dominios/Edit/5
        public ActionResult Edit(int? id)
        {
            return null;
            /*
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            identifierDomain identifierDomain = db.identifierDomains.Find(id);
            if (identifierDomain == null)
            {
                return HttpNotFound();
            }
            return View(identifierDomain);*/
        }

        // POST: dominios/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "identifierDomainId,dateCreated,dateCreatedSpecified,identifierDomainDescription,identifierDomainIdSpecified,identifierDomainName,namespaceIdentifier,universalIdentifier,universalIdentifierTypeCode")] identifierDomain identifierDomain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(identifierDomain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(identifierDomain);
        }

        // GET: dominios/Delete/5
        public ActionResult Delete(string dominio)
        {
            Debug.WriteLine("Eliminando dominio...");
            Debug.WriteLine(dominio);
            try
            {
                f.adminController.dropSchema(dominio);
            }
            catch (Exception e)
            {
                throw e;
                Debug.WriteLine("Error eliminar dominio....");
            }

            return Json("ok", JsonRequestBehavior.AllowGet);//View(identifierDomain);
        }

        // POST: dominios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*
            identifierDomain identifierDomain = db.identifierDomains.Find(id);
            db.identifierDomains.Remove(identifierDomain);
            db.SaveChanges(); */
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
