using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            
            foreach (String kljuc in forma.Keys)
            {
                Response.WriteAsync("kljuc: " + kljuc + "  -   vrijednost:   " + forma[kljuc] + " --- ");
            }
            return null;
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