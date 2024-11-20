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
                UsuarioLogado.Role = _userManager.GetRolesAsync(UsuarioLogado).Result.FirstOrDefault();

                if (UsuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == UsuarioLogado.Medico.Id && x.Data.Date == DataEvento).ToList());
                else if (UsuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.Data.Date == DataEvento).ToList());

                foreach(var item in consultas)
                {
                    if(item.StatusConsulta == (int)StatusConsulta.Confirmada && item.Data <= DateTime.Now)
                    {
                        item.StatusConsulta = (int)StatusConsulta.EmAndamento;
                    }

                }
                SaveChanges();
            }

            ViewBag.Pacientes = Contexto<Paciente>().ToList();
            ViewBag.Medicos = Contexto<Medico>().Include(x => x.Usuario).ToList();
            return PartialView("Views\\Consulta\\_EventosDia.cshtml", consultas);
        }


        [HttpPost]
        public async Task<ActionResult> Salvar(Consulta consulta)
        {
            ModelState.Clear();
            ValidarCamposConsulta(consulta);

            if (!ModelState.IsValid)
            {
                return View("Views\\Consulta\\index.cshtml", new Consulta());
            }
            consulta.StatusConsulta = (int)StatusConsulta.AguardandoConfirmacao;
            consulta.Data = consulta.Data.AddHours(consulta.Hora.Hours);
            consulta.Data = consulta.Data.AddMinutes(consulta.Hora.Minutes);

            Contexto<Consulta>().Add(consulta);

            SaveChanges();

            DisplayMensagemSucesso();
            //return View("Views\\Consulta\\Index.cshtml", new List<Cons>consulta);

            return RedirectToAction("Index", "Consulta");
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

        [HttpPost]
        public async Task<ActionResult> ConfirmarConsulta(int idConsulta)
        {
            var consulta = Contexto<Consulta>().FirstOrDefault(x => x.Id == idConsulta);
            consulta.StatusConsulta = (int)StatusConsulta.Confirmada;            

            SaveChanges();
            return View("Views\\Consulta\\Index.cshtml", consulta);
        }

        public void ValidarCamposConsulta(Consulta consulta)
        {
            if (consulta.IdMedico == 0)
                ModelState.AddModelError("Médico", "O campo é obrigatório");
            if (consulta.IdPaciente == 0)
                ModelState.AddModelError("Médico", "O campo é obrigatório");
            if (string.IsNullOrEmpty(consulta.Motivo))
                ModelState.AddModelError("Motivo", "O campo é obrigatório");
            if (consulta.Hora == new TimeSpan())
                ModelState.AddModelError("Horário da Consulta", "O campo é obrigatório");

        }
    }
}
