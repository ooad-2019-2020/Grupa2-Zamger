using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ZamgerV2_Implementation.Helpers;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class PočetniController : Controller
    {
        public PočetniController()
        {
        }

        [Route("/Home")]
        [Route("~/")]
        [Route("/Home/Index")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [Route("/Home")]
        [Route("~/")]
        [Route("/Home/Index")]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (String.IsNullOrEmpty(model.username) || String.IsNullOrEmpty(model.password))
            {
                return View(model);
            }

            string sqlKveri = "SELECT idKorisnika, tipKorisnika from korisnici where username like @user and password like @pass";

            SqlConnection conn = new SqlConnection("server=DESKTOP-0G31M9N;database=zamgerDB-new;Trusted_Connection=true");

            SqlCommand command = new SqlCommand(sqlKveri, conn);

            var userParam = new SqlParameter("user", System.Data.SqlDbType.NVarChar);
            userParam.Value = model.username;
            var passParam = new SqlParameter("pass", System.Data.SqlDbType.NVarChar);
            passParam.Value = model.password;

            command.Parameters.Add(userParam);
            command.Parameters.Add(passParam);
            try
            {
                conn.Open();
                var result = command.ExecuteReader();
                if (!result.HasRows)
                {
                    conn.Close();
                    ModelState.AddModelError("", "username ili password nisu tačni");
                    return View(model);
                }
                else
                {
                    result.Read();
                    int idKorisnika = result.GetInt32(0);
                    TipKorisnika tipKorisnika = (TipKorisnika)result.GetInt32(1);

                    Autentifikacija.PokreniNovuSesiju(idKorisnika, HttpContext, tipKorisnika);

                    if (tipKorisnika == TipKorisnika.StudentskaSluzba)
                    {
                        return RedirectToAction("Dashboard", new RouteValueDictionary(
                        new { controller = "Studentska", action = "Dashboard" }));
                    }
                    else if (tipKorisnika == TipKorisnika.Student)
                    {
                        return RedirectToAction("Dashboard", new RouteValueDictionary(
                        new { controller = "Student", action = "Dashboard"}));
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", new RouteValueDictionary(
                        new { controller = "NastavnoOsoblje", action = "Dashboard"}));
                    }
                }
            }
            catch (Exception)
            {
                return RedirectToAction("OdjaviSe");
            }
        }

        [Route("Početni/OdjaviSe")]
        public IActionResult OdjaviSe()
        {
            Autentifikacija.OcistiSesiju(HttpContext);
            return RedirectToAction(nameof(Login));
        }


        [Route("/pristup-odbijen")]
        public IActionResult pristupOdbijen(int idGreške)
        {
            return View();
        }

        //ovdje treba dodati neki view za errore, koji će primati neki parametar npr tipErrora a mi ćemo shodno tome neki custom error view baciti
    }
}
