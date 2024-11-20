using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class LoginController : GlobalController
    {
        public LoginController(AppDbContext context) : base(context)
        {
        }

        public ActionResult Index()
        {

            // Log de início de acesso à página Index
            Log logIndex = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
                Alteracao = "Início de acesso à página Index"
            };
            //Contexto<Log>().Add(logIndex); // Adicionando log

            return View();
        }

        public ActionResult Editar()
        {
            // Log de início de edição de usuário
            Log logEditar = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
                Alteracao = "Início de edição de usuário"
            };
            //Contexto<Log>().Add(logEditar); // Adicionando log

            return View();
        }

        public IActionResult Privacy()
        {
            // Log de acesso à página Privacy
            Log logPrivacy = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
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
                TabelaReferencia = "Login",
                Alteracao = "Erro no processo de login"
            };
            //Contexto<Log>().Add(logError); // Adicionando log

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
