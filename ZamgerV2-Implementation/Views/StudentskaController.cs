using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ZamgerV2_Implementation.Views
{
    public class StudentskaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}