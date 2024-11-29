using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // Importar o namespace para o IMemoryCache
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class LoginController : GlobalController
    {
        private readonly IMemoryCache _cache; // Declarando a depend�ncia de cache

        // Injeta a depend�ncia de IMemoryCache no construtor
        public LoginController(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
            _cache = cache; // Inicializando o cache
        }

        public ActionResult Index()
        {
            // Log de in�cio de acesso � p�gina Index
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logIndex = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "In�cio de acesso � p�gina Index",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logIndex); // Adicionando log
                SaveChanges();
            }

            return View();
        }

        public ActionResult Editar()
        {
            // Log de in�cio de edi��o de usu�rio
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logEditar = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "In�cio de edi��o de usu�rio",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logEditar); // Adicionando log
                SaveChanges();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            // Log de acesso � p�gina Privacy
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logPrivacy = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "Acesso � p�gina de Privacidade",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logPrivacy); // Adicionando log
                SaveChanges();
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Log de erro
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logError = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "Erro no processo de login",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logError); // Adicionando log
                SaveChanges();
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
