using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class StudentskaController : Controller
    {
        [Route("/studentska/dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Route("/studentska/svi-studenti")]
        public IActionResult AllStudents()
        {
            return View();
        }

        [Route("/studentska/svi-studenti/kreiraj-studenta")]
        [Route("/studentska/kreiraj-studenta")]
        [HttpGet]
        public IActionResult KreirajStudenta()
        {
            return View();
        }


        [Route("/studentska/svi-studenti/kreiraj-studenta")]
        [Route("/studentska/kreiraj-studenta")]
        [HttpPost]
        public IActionResult KreirajStudenta(IFormCollection forma)
        {
            Logger logg = Logger.GetInstance();
            if (forma["BScInfo"].Equals("3")) //ako osoba nije završila BSc tada se upisuje kao običan a ne MasterStudent -> nema potrebe za ekvivalentiranjem
            {
                if (!forma["izabraniSmjer"].Equals("Izaberite smjer")) 
                { 
                    Student noviStudent = new Student(forma["ime"], forma["prezime"], forma["datumRodjenja"], forma["prebivaliste"], null, null, forma["izabraniSpol"], forma["izabraniSmjer"], null);
                    if (validirajStudenta(noviStudent))
                    {
                        logg.generišiKorisničkePodatke(noviStudent);
                        Response.WriteAsync("Sve je OK");
                    }
                    else
                    {
                        Response.WriteAsync("Nije OK - student -> ima nepravilne podatke");
                    }
                }
                else
                {
                    Response.WriteAsync("Nije OK - taj smjer ne postoji");
                }
            }
            else
            {
                Response.WriteAsync("Nije OK");
            }
            return null;
        }


        private bool validirajStudenta(Student noviStudent)
        {
            if (noviStudent.Ime == null || noviStudent.Prezime == null || noviStudent.DatumRođenja == null || noviStudent.MjestoPrebivališta == null || noviStudent.Odsjek == null || noviStudent.Odsjek.Equals("Izaberite smjer")) return false;
            return true;
        }

        [Route("/studentska/svo-nastavno-osoblje")]
        public IActionResult AllTeachers()
        {
            return View();

        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpGet]
        public IActionResult KreirajNastavnoOsoblje()
        {
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        [HttpPost]
        public IActionResult KreirajNastavnoOsoblje(IFormCollection forma)
        {
            foreach(String kljuc in forma.Keys){
                Response.WriteAsync("kljuc: " + kljuc + "  -   vrijednost:   " + forma[kljuc]+" --- ");
            }
            return View();
        }

        [Route("/studentska/svi-studenti-list")]
        public IActionResult AllStudentsList()
        {
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje-list")]
        public IActionResult AllTeachersList()
        {
            return View();
        }
    }
}