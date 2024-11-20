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
            // Log de início de acesso à página Index
            Log logIndex = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = "Início de acesso à página Index"
            };
            //Contexto<Log>().Add(logIndex); // Adicionando log

            var paciente = Contexto<Paciente>().FirstOrDefault(p => p.Id == 1);

            return View("Views\\login\\index.cshtml");
        }

        public IActionResult Privacy()
        {
            // Log de acesso à página Privacy
            Log logPrivacy = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = "Acesso à página de Privacidade"
            };
            //Contexto<Log>().Add(logPrivacy); // Adicionando log

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Log de erro
            Log logError = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = "Erro no processo"
            };
            //Contexto<Log>().Add(logError); // Adicionando log

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
