using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class NastavnoOsobljeController : Controller
    {
        private NastavnoOsoblje trenutniKorisnik;
        public IActionResult Dashboard(int id)
        {

            return View();
        }
    }
}