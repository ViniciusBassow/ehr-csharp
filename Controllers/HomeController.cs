using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class HomeController : Controller
    {
        AppDbContext _dbContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext dbContext, ILogger<HomeController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public ActionResult Index()
        {
            List<Paciente> pacientes = new List<Paciente>();
            pacientes = _dbContext.Paciente.ToList();

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
