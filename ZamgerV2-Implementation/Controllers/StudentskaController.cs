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
    }
}