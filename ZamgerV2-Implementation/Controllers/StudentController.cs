using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class StudentController : Controller
    {

        private Student trenutniKorisnik;
        private ZamgerDbContext zmgr;

        public StudentController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }
        [Route("/student/dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if(tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.polozeni = zmgr.dajBrojPoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.nepolozeni = zmgr.dajBrojNepoloženihPredmeta(trenutniKorisnik.BrojIndeksa);
            ViewBag.prosjek = zmgr.dajProsjekPoID(trenutniKorisnik.BrojIndeksa);
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            //ViewBag.listaPredmeta = zmgr.dajMojePredmete(trenutniKorisnik.BrojIndeksa);
            

            return View(trenutniKorisnik); 
        }

        [Route("/student/kreiraj-zahtjev/{id}")]
        [HttpGet]
        public IActionResult KreirajZahtjev(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/kreiraj-zahtjev/{id}")]
        [HttpPost]
        public IActionResult KreirajZahtjev(int id, IFormCollection forma)
        {
            int idZaht = zmgr.generišiIdZahtjeva();
            if(zmgr.spremiZahtjev(new Zahtjev(id, forma["VrstaZahtjeva"].ToString(), DateTime.Now, 0, idZaht)))
            {
                return RedirectToAction("UspješnoKreiranZahtjev", new { id = id });
            }
            else
            {
                return null; //ovdje treba neki error view vratit 404 il nešta
            }
        }


        [Route("/student/uspjesno-poslan-zahtjev/{id}")]
        [HttpGet]
        public IActionResult UspješnoKreiranZahtjev(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }

        [Route("/student/moji-zahtjevi/{id}")]
        [HttpGet]
        public IActionResult MojiZahtjevi(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.mojiZahtjevi = zmgr.dajZahtjeveZaStudenta(id);
            return View(trenutniKorisnik);
        }

        [Route("/student/sva-obavještenja-list/{id}")]
        public IActionResult AllStudentAnnouncementsList(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }

            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }


        [Route ("/student/obavještenje/{idObavještenja}/{id}")]
        public IActionResult AnnouncementStudentInfo(int idObavještenja, int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.obavještenje = zmgr.dajObavještenjePoId(idObavještenja);
            return View(trenutniKorisnik);
        }

        [Route ("/student/predmet/{id}/{idPredmeta}/{studijskaGodina}")]
        public IActionResult StudentSubjectInfo(int id, int idPredmeta, int studijskaGodina)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.predmet = zmgr.dajPredmetZaStudentaPoID(id, idPredmeta, studijskaGodina);
            //ViewBag.listaPredmeta = zmgr.dajMojePredmete(id);

            return View(trenutniKorisnik);
        }


        [Route ("/student/predmeti-list/{id}")]
        public IActionResult MySubjects(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            ViewBag.listaPredmeta = trenutniKorisnik.Predmeti;

            return View(trenutniKorisnik);
        }

        [Route ("/student/poruke/moj-inbox/{idStudenta}")]
        public IActionResult mojInbox(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }
        [Route("/student/poruke/moj-outbox/{idStudenta}")]
        public IActionResult mojOutbox(int idStudenta)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(idStudenta);
            if (tempK.GetType() == typeof(Student))
            {
                trenutniKorisnik = (Student)tempK;
            }
            else
            {
                trenutniKorisnik = (MasterStudent)tempK;
            }
            return View(trenutniKorisnik);
        }





    }
}