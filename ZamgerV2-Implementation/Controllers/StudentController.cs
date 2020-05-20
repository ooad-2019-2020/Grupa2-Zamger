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
        [Route("/student/{id}/dashboard")]
        public IActionResult Dashboard(int id)
        {
            trenutniKorisnik = zmgr.dajStudentaPoID(id);
            trenutniKorisnik.Predmeti = zmgr.formirajPredmeteZaStudentaPoId(id);
            foreach (PredmetZaStudenta prdmt in trenutniKorisnik.Predmeti)
            {
                prdmt.Aktivnosti = zmgr.dajAktivnostiZaStudentovPredmet(prdmt.IdPredmeta, prdmt.IdStudenta);
            }
            Response.WriteAsync("STUDENT USPJEŠNO LOGOVAN: "+trenutniKorisnik.Ime+" "+trenutniKorisnik.Prezime+" "+trenutniKorisnik.Predmeti.Count);
            return View(); //View ovaj nije napravljen potrebno je sada fino izdijanirati UI, odnosno studentske viewe na sistem!
        }
    }
}