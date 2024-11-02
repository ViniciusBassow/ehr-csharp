using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class ConsultaController : GlobalController
    {
        private readonly IMemoryCache _cache;
        public ConsultaController(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        //wwwroot\Calendar\fullcalendar\packages\core\main.js           -> CONTROLE CALENDARIUO
        //wwwroot\Calendar\fullcalendar\packages\core\main.css          -> ESTILOS
        public ActionResult Index()
        {
            List<Consulta> consultas = new List<Consulta>();
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                if (UsuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == UsuarioLogado.Medico.Id).ToList());
                else if (UsuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).ToList());
            }

            return View("Views\\Consulta\\Index.cshtml", consultas);
        }




        [HttpPost]
        public async Task<ActionResult> Salvar(Consulta consulta)
        {


            if (!ModelState.IsValid)
            {
                return View("Views\\Usuario\\editar.cshtml", new Consulta());
            }

            Contexto<Consulta>().Add(consulta);

            SaveChanges();
            return View("Views\\Consulta\\Index.cshtml", consulta);
        }
    }
}
