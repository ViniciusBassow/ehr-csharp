using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class UsuarioController : GlobalController
    {
        public UsuarioController(AppDbContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            List<Usuario> usuarios = Contexto<Usuario>().ToList();
            
            return View(usuarios);
        }

        public ActionResult Editar()
        {

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
