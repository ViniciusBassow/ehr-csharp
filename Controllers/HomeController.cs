using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory; // Importando o namespace para IMemoryCache
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class HomeController : GlobalController
    {
        private readonly IMemoryCache _cache; // Declarando o cache

        // Injeta a dependência de IMemoryCache no construtor
        public HomeController(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache; // Inicializa o cache
        }

        public ActionResult Index()
        {
            // Log de início de acesso à página Index
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logIndex = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = "Início de acesso à página Index",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logIndex); // Adicionando log
                SaveChanges();
            }

            var paciente = Contexto<Paciente>().FirstOrDefault(p => p.Id == 1);

            return View("Views\\login\\index.cshtml");
        }

        public IActionResult Privacy()
        {
            // Log de acesso à página Privacy
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logPrivacy = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
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
                    TabelaReferencia = "Usuario",
                    Alteracao = "Erro no processo",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logError); // Adicionando log
                SaveChanges();
            }

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
