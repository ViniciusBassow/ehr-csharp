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

            // Log de in�cio de acesso � p�gina Index
            Log logIndex = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
                Alteracao = "In�cio de acesso � p�gina Index"
            };
            //Contexto<Log>().Add(logIndex); // Adicionando log

            return View();
        }

        public ActionResult Editar()
        {
            // Log de in�cio de edi��o de usu�rio
            Log logEditar = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
                Alteracao = "In�cio de edi��o de usu�rio"
            };
            //Contexto<Log>().Add(logEditar); // Adicionando log

            return View();
        }

        public IActionResult Privacy()
        {
            // Log de acesso � p�gina Privacy
            Log logPrivacy = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Login",
                Alteracao = "Acesso � p�gina de Privacidade"
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
