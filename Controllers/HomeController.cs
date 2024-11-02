using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;
using Serilog; // Adicione esta linha para usar o Serilog

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
            Log.Information("Acessando a p�gina inicial. Paciente encontrado: {@Paciente}", paciente); // Log de informa��o ao acessar a p�gina inicial

            return View("Views\\login\\index.cshtml");
        }

        public IActionResult Privacy()
        {
            Log.Information("Acessando a p�gina de privacidade."); // Log de informa��o ao acessar a p�gina de privacidade
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var errorViewModel = new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier };
            Log.Error("Ocorreu um erro. RequestId: {RequestId}", errorViewModel.RequestId); // Log de erro quando ocorre um erro
            return View(errorViewModel);
        }
    }
}
