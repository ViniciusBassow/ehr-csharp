using ehr_csharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;

namespace ehr_csharp.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdministratorController : GlobalController
    {
        private readonly IMemoryCache _cache;

        public AdministratorController(AppDbContext context, IMemoryCache cache) : base(context, cache)
        {
            _cache = cache; // Inicializando o cache
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> Usuarios()
        {
            return View();
        }

        public async Task<ActionResult> Calendario()
        {
            return View();
        }


        public async Task<ActionResult> Relatorios()
        {
            return View();
        }

        public async Task<ActionResult> Configuracoes()
        {
            var configuracoes = Contexto<Config>().ToList();

            return View(configuracoes);
        }

        public async Task<ActionResult> dash()
        {
            return View();
        }

        public async Task<ActionResult> reports()
        {
            return View();
        }

        public async Task<ActionResult> configurações_do_sistema()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Salvar(List<Config> configs)
        {
            if (configs != null)
            {
                var configBd = Contexto<Config>().ToList();


                // Processar a lista de configurações aqui
                foreach (var config in configs)
                {
                    var tempo = configBd.FirstOrDefault(x => x.IdConfig == config.IdConfig);
                    tempo.Valor = config.Valor;

                }
            }
            SaveChanges();
            return RedirectToAction("Configuracoes");
        }


    }
}
