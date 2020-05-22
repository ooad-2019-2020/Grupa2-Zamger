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

    }
}