using DA2_SistemaEscolar2016.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DA2_SistemaEscolar2016.Controllers
{
    public class AlumnoController : Controller
    {
        //conectarnos a la BD
        ApplicationDbContext db = new ApplicationDbContext();
        [HttpPost]
        public ActionResult Listar(string nombreBuscado)
        {
            var resultadoBusqueda = new List<Alumno>();
            if (!string.IsNullOrEmpty(nombreBuscado))
            {
                resultadoBusqueda = db.alumnos.Where(alumno => alumno.apellidoMaterno.Contains(nombreBuscado) || alumno.apellidoPaterno.Contains(nombreBuscado) || alumno.nombre.Contains(nombreBuscado)).ToList();

            }
            else
            {
                resultadoBusqueda = db.alumnos.ToList();
            }

            return View(resultadoBusqueda);
        }
        
           [HttpGet]
            public ActionResult Listar()
        {
            //consultar la lista de alumnos
            //select * FROM alumnos
            var  todosLosAlumnos = db.alumnos.ToList();

            //pedirle a la lista que muestre los resultados en pantalla

            return View(todosLosAlumnos);
        }
        [HttpGet]
        //este te muestra con la pagina con la forma para dar de alta aun nuevo alumno
        [Authorize(Roles = "Admin, Capturista")]
        public ActionResult crear()
        {
            //tomar los datos con los que rellenas la lista
            var grupos = db.grupos;
            //crearla lista de seleccion
            SelectList grupoID = new SelectList(grupos, "grupoID", "nombreGrupo");
            ViewBag.grupoID = grupoID;




            return View();
        }

        //este se encarga de recibir los datos del nuevo alumno y guardarlo
        [HttpPost]
        [Authorize(Roles = "Admin, Capturista")]
        public ActionResult crear(Alumno alumnoNuevo, bool enDetallesDelGrupo = false)
        {

            if (ModelState.IsValid)
            {
                //Crear Alumno
                db.alumnos.Add(alumnoNuevo);

                //Guardar Cambios
                db.SaveChanges();
                if (enDetallesDelGrupo)
                {
                    return RedirectToAction("detalles", "grupo", new { id = alumnoNuevo.grupoID });
                }

                else
                {
                    return RedirectToAction("listar");
                }

            }
            //si lleas tan lejos es que hay un problema
            ViewBag.MensajeError = "Hubo un error favor de verificar la informacion";

            //te devuelve a intentarlo de nuevo
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult eliminar(int id=0)
        {
            //se busca el alumno que se quiere eliminar
            var alumno = db.alumnos.Find(id);
            if (alumno == null)
            {
                return RedirectToAction("listar");
            }
            return View(alumno);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ActionName("eliminar")]
        public ActionResult ConfirmarEliminar(int numeroMatricula = 0)
        {
            //se busca el alumno que se quiere eliminar
            Alumno alumno = db.alumnos.Find(numeroMatricula);
            if (alumno == null)
            {
                return RedirectToAction("listar");
            }


            //elimina al alumno
            db.alumnos.Remove(alumno);
            //ejecuta la lista que querias en la BD
            db.SaveChanges();
            return RedirectToAction("listar");
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Capturista")]
        public ActionResult editar(int id= 0)
        {
            var alumno = db.alumnos.Find(id);
            if (alumno == null)
            {
                return RedirectToAction("listar");
            }

            //tomar los datos con los que rellenas la lista
            var grupos = db.grupos;
            //crearla lista de seleccion
            SelectList grupoID = new SelectList(grupos, "grupoID", "nombreGrupo");
            ViewBag.grupoID = grupoID;

            return View(alumno);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Capturista")]
        public ActionResult editar(Alumno alumnoEditado)
        {
            if (ModelState.IsValid)
            {
                // unica forma de modificar el registro actual
                db.Entry(alumnoEditado).State = EntityState.Modified;
                db.SaveChanges();
                //si llega aqui todo va bien
                return RedirectToAction("listar");
            }
            
                   
            return View();
        }
    }

   

}