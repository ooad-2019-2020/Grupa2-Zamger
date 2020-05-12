using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult KreirajStudenta()
        {
            return View();
        }

        [Route("/studentska/svo-nastavno-osoblje")]
        public IActionResult AllTeachers()
        {
            return View();

        }

        [Route("/studentska/svo-nastavno-osoblje/kreiraj-nastavno-osoblje")]
        [Route("/studentska/kreiraj-nastavno-osoblje")]
        public IActionResult KreirajNastavnoOsoblje()
        {
            return View();
        }
    }
}