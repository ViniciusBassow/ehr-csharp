using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;
using Serilog; // Adicione esta linha para usar o Serilog

namespace ehr_csharp.Controllers
{
    public class LoginController : GlobalController
    {
        public LoginController(AppDbContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            Log.Information("Acessando a p�gina de login."); // Log de informa��o ao acessar a p�gina de login
            return View();
        }

        public ActionResult Editar()
        {
            Log.Information("Acessando a p�gina de edi��o de login."); // Log de informa��o ao acessar a p�gina de edi��o
            return View();
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
            Log.Error("Ocorreu um erro ao acessar a p�gina. RequestId: {RequestId}", errorViewModel.RequestId); // Log de erro quando ocorre um erro
            return View(errorViewModel);
        }
    }
}
