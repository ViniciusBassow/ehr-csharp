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
        private readonly UserManager<Usuario> _userManager;
        public ConsultaController(AppDbContext context, IMemoryCache cache, UserManager<Usuario> userManager) : base(context)
        {
            _cache = cache;
            _userManager = userManager;
        }

        //wwwroot\Calendar\fullcalendar\packages\core\main.js           -> CONTROLE CALENDARIUO
        //wwwroot\Calendar\fullcalendar\packages\core\main.css          -> ESTILOS
        public ActionResult Index()
        {
            List<Consulta> consultas = new List<Consulta>();
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                UsuarioLogado.Role = _userManager.GetRolesAsync(UsuarioLogado).Result.FirstOrDefault();

                if (UsuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == UsuarioLogado.Medico.Id).ToList());
                else if (UsuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).ToList());
            }

            return View("Views\\Consulta\\Index.cshtml", consultas);
        }

        public async Task<ActionResult> ExibirEventosDia(DateTime DataEvento)
        {
            List<Consulta> consultas = new List<Consulta>();
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                //var roles = await _userManager.GetRolesAsync(UsuarioLogado);
                UsuarioLogado.Role = _userManager.GetRolesAsync(UsuarioLogado).Result.FirstOrDefault();

                if (UsuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == UsuarioLogado.Medico.Id && x.Data.Date == DataEvento).ToList());
                else if (UsuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.Data.Date == DataEvento).ToList());
            }

            return PartialView("Views\\Consulta\\_EventosDia.cshtml", consultas);
        }


        [HttpPost]
        public async Task<ActionResult> Salvar(Consulta consulta)
        {
            if (!ModelState.IsValid)
            {
                return View("Views\\Consulta\\index.cshtml", new Consulta());
            }

            Contexto<Consulta>().Add(consulta);

            SaveChanges();
            return View("Views\\Consulta\\Index.cshtml", consulta);
        }

        [HttpPost]
        public async Task<ActionResult> CancelarConsulta(int idConsulta, string motivoCancelamento)
        {
            var consulta = Contexto<Consulta>().FirstOrDefault(x => x.Id == idConsulta);
            consulta.StatusConsulta = (int)StatusConsulta.Cancelado;
            consulta.MotivoCancelamento = motivoCancelamento;

            SaveChanges();
            return View("Views\\Consulta\\Index.cshtml", consulta);
        }
    }
}
