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
        private readonly IMemoryCache _cache; // Declarando a dependência de cache

        // Injeta a dependência de IMemoryCache no construtor
        public LoginController(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
            _cache = cache; // Inicializando o cache
        }

        public ActionResult Index()
        {
            // Log de início de acesso à página Index
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logIndex = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "Início de acesso à página Index",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logIndex); // Adicionando log
                SaveChanges();
            }

            return View();
        }

        public ActionResult Editar()
        {
            // Log de início de edição de usuário
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logEditar = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "Início de edição de usuário",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logEditar); // Adicionando log
                SaveChanges();
            }

            return View();
        }

        public IActionResult Privacy()
        {
            // Log de acesso à página Privacy
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logPrivacy = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Login",
                    Alteracao = "Acesso à página de Privacidade",
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
