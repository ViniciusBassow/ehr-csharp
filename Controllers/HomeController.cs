using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class HomeController : GlobalController
    {
        public HomeController(AppDbContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            var paciente = Contexto<Paciente>().FirstOrDefault(p => p.Id == 1);

            return View("Views\\account-billing.cshtml");
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
