using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class NastavnoOsobljeController : Controller
    {
        private NastavnoOsoblje trenutniKorisnik;
        private ZamgerDbContext zmgr;


        public NastavnoOsobljeController()
        {
            zmgr = ZamgerDbContext.GetInstance();
        }

        [Route("/nastavno-osoblje/dashboard/{id}")]
        public IActionResult Dashboard(int id)
        {
            KreatorKorisnika creator = new KreatorKorisnika();
            Korisnik tempK = creator.FactoryMethod(id);
            if (tempK.GetType() == typeof(NastavnoOsoblje))
            {
                trenutniKorisnik = (NastavnoOsoblje)tempK;
            }
            else
            {
                trenutniKorisnik = (Profesor)tempK;
            }
            ViewBag.listaObavjestenja = zmgr.dajSvaObavještenja();
            return View(trenutniKorisnik);
        }
    }
}