using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ZamgerV2_Implementation.Models;

namespace ZamgerV2_Implementation.Controllers
{
    public class PočetniController : Controller
    {
        private readonly ILogger<PočetniController> _logger;

        public PočetniController(ILogger<PočetniController> logger)
        {
            _logger = logger;
        }
         
        [Route("/Home")]
        [Route("~/")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [Route("/Home")]
        [Route("~/")]
        [Route("/Home/Index")]
        [HttpPost]
        public IActionResult Login(IFormCollection kolekcija)
        {
            String username = kolekcija["username"];
            String password = kolekcija["password"];
            if(username==null || password == null)
            {
                Response.WriteAsync("username ili password polje prazno!");
            }


            string sqlKveri = "SELECT idKorisnika, tipKorisnika from korisnici where username like @user and password like @pass";

           SqlConnection conn = new SqlConnection("server=DESKTOP-0G31M9N;database=zamgerDB-new;Trusted_Connection=true");
           SqlCommand command = new SqlCommand(sqlKveri, conn);
            
                var userParam = new SqlParameter("user", System.Data.SqlDbType.NVarChar);
                userParam.Value = username;
                 var passParam = new SqlParameter("pass", System.Data.SqlDbType.NVarChar);
                 passParam.Value = password;

            command.Parameters.Add(userParam);
            command.Parameters.Add(passParam);
            try
            {
                conn.Open();
                var result = command.ExecuteReader();
                if (!result.HasRows)
                {
                    Response.WriteAsync("Korisnik ne postoji u sistemu ili pogresno uneseni podaci!");
                    conn.Close();
                }
                else
                {
                    result.Read();
                    int idKorisnika = result.GetInt32(0);
                    int tipKorisnika = result.GetInt32(1);
                    if(tipKorisnika == 3)
                    {
                        return RedirectToAction("Dashboard", new RouteValueDictionary(
                        new { controller = "Studentska", action = "Dashboard"}));
                    }
                    else if(tipKorisnika == 1)
                    {
                        return RedirectToAction("Dashboard", new RouteValueDictionary(
                        new { controller = "Student", action = "Dashboard", id=idKorisnika}));
                    }
                    else
                    {

                    }
                    
                    return RedirectToAction("Dashboard", new RouteValueDictionary(
                    new { controller = "NastavnoOsoblje", action = "Dashboard"}));
                }
            }
            catch (Exception e)
            {
                Response.WriteAsync(e.StackTrace);
                conn.Close();
            }
            return null;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //ovdje treba dodati neki view za errore, koji će primati neki parametar npr tipErrora a mi ćemo shodno tome neki custom error view baciti
    }
}
