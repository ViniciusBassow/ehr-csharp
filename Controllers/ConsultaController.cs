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
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                usuarioLogado.Role = _userManager.GetRolesAsync(usuarioLogado).Result.FirstOrDefault();

                if (usuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == usuarioLogado.Medico.Id).ToList());
                else if (usuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).ToList());
            }

            // Log de exibição da página de consultas
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogadoLog))
            {
                Log logExibirConsultas = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Consulta",
                    Alteracao = "Exibição da lista de consultas",
                    IdUsuarioAlteracao = usuarioLogadoLog.Id
                };

                Contexto<Log>().Add(logExibirConsultas); // Adicionando log
                SaveChanges();
            }

            return View("Views\\Consulta\\Index.cshtml", consultas);
        }

        public async Task<ActionResult> ExibirEventosDia(DateTime DataEvento)
        {
            List<Consulta> consultas = new List<Consulta>();
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                usuarioLogado.Role = _userManager.GetRolesAsync(usuarioLogado).Result.FirstOrDefault();

                if (usuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == usuarioLogado.Medico.Id && x.Data.Date == DataEvento).ToList());
                else if (usuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.Data.Date == DataEvento).ToList());

                foreach (var item in consultas)
                {
                    if (item.StatusConsulta == (int)StatusConsulta.Confirmada && item.Data <= DateTime.Now)
                    {
                        item.StatusConsulta = (int)StatusConsulta.EmAndamento;
                    }
                }

                SaveChanges();

                if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogadoEventos))
                {
                    Log logExibirEventosDia = new Log()
                    {
                        DataAlteracao = DateTime.Now,
                        TabelaReferencia = "Consulta",
                        Alteracao = $"Exibição dos eventos do dia {DataEvento:dd/MM/yyyy}",
                        IdUsuarioAlteracao = usuarioLogadoEventos.Id
                    };

                    Contexto<Log>().Add(logExibirEventosDia); // Adicionando log
                    SaveChanges();
                }
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
            // Log de criação ou atualização de consulta
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logSalvarConsulta = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Consulta",
                    Alteracao = consulta.Id == 0
                        ? $"Criação de nova consulta para o paciente {consulta.IdPaciente}"
                        : $"Atualização de consulta para o paciente {consulta.IdPaciente}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logSalvarConsulta); // Adicionando log
                SaveChanges();
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

            // Log de cancelamento de consulta
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logCancelarConsulta = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Consulta",
                    Alteracao = $"Consulta cancelada para o paciente {consulta.IdPaciente} - Motivo: {motivoCancelamento}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logCancelarConsulta); // Adicionando log
                SaveChanges();
            }
            SaveChanges();
            return View("Views\\Consulta\\Index.cshtml", consulta);
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmarConsulta(int idConsulta)
        {
            var consulta = Contexto<Consulta>().FirstOrDefault(x => x.Id == idConsulta);
            consulta.StatusConsulta = (int)StatusConsulta.Confirmada;

            // Log de confirmação de consulta
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logConfirmarConsulta = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Consulta",
                    Alteracao = $"Consulta confirmada para o paciente {consulta.IdPaciente}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logConfirmarConsulta); // Adicionando log
                SaveChanges();
            }


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

        [HttpPost]
        public async Task<JsonResult> SalvarAbaHemograma(Hemograma hemograma)
        {
            ModelState.Clear();
            hemograma.Consulta = await Contexto<Consulta>().FirstOrDefaultAsync(x => x.Id == hemograma.IdConsulta);


            var hemogramaDb = await Contexto<Hemograma>().FirstOrDefaultAsync(x => x.IdConsulta == hemograma.IdConsulta);

            if (hemogramaDb == null)
            {
                Contexto<Hemograma>().Add(hemograma);

                if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
                {
                    Log logCriacaoHemograma = new Log()
                    {
                        DataAlteracao = DateTime.Now,
                        TabelaReferencia = "Hemograma",
                        Alteracao = $"Criação de novo hemograma para consulta ID {hemograma.IdConsulta}",
                        IdUsuarioAlteracao = UsuarioLogado.Id
                    };

                    Contexto<Log>().Add(logCriacaoHemograma); // Adicionando log
                    SaveChanges();
                }
            }
            else
            {
                hemogramaDb.Eritrocitos = hemograma.Eritrocitos;
                hemogramaDb.Hemoglobina = hemograma.Hemoglobina;
                hemogramaDb.Hematocrito = hemograma.Hematocrito;
                hemogramaDb.VCM = hemograma.VCM;
                hemogramaDb.HCM = hemograma.HCM;
                hemogramaDb.CHCM = hemograma.CHCM;
                hemogramaDb.RDW = hemograma.RDW;
                hemogramaDb.Leucocitos_Absoluto = hemograma.Leucocitos_Absoluto;
                hemogramaDb.Leucocitos_Relativo = hemograma.Leucocitos_Relativo;
                hemogramaDb.Bastonetes_Absoluto = hemograma.Bastonetes_Absoluto;
                hemogramaDb.Bastonetes_Relativo = hemograma.Bastonetes_Relativo;
                hemogramaDb.Segmentados_Absoluto = hemograma.Segmentados_Absoluto;
                hemogramaDb.Segmentados_Relativo = hemograma.Segmentados_Relativo;
                hemogramaDb.Eosinofilos_Relativo = hemograma.Eosinofilos_Relativo;
                hemogramaDb.Eosinofilos_Absoluto = hemograma.Eosinofilos_Absoluto;
                hemogramaDb.Basofilos_Absoluto = hemograma.Basofilos_Absoluto;
                hemogramaDb.Basofilos_Relativo = hemograma.Basofilos_Relativo;
                hemogramaDb.Linfocitos_Absoluto = hemograma.Linfocitos_Absoluto;
                hemogramaDb.Linfocitos_Relativo = hemograma.Linfocitos_Relativo;
                hemogramaDb.Monocitos_Relativo = hemograma.Monocitos_Relativo;
                hemogramaDb.Monocitos_Absoluto = hemograma.Monocitos_Absoluto;
                hemogramaDb.Plaquetas = hemograma.Plaquetas;
                hemogramaDb.VPM = hemograma.VPM;
                hemogramaDb.Glicemia = hemograma.Glicemia;
                hemogramaDb.Creatinina = hemograma.Creatinina;
                hemogramaDb.AcidoUrico = hemograma.AcidoUrico;
                hemogramaDb.Prolactina = hemograma.Prolactina;
                hemogramaDb.Testosterona = hemograma.Testosterona;
                hemogramaDb.ColesterolTotal = hemograma.ColesterolTotal;
                hemogramaDb.HDL = hemograma.HDL;
                hemogramaDb.Triglicerides = hemograma.Triglicerides;
                hemogramaDb.LDL = hemograma.LDL;
                hemogramaDb.NaoHDL = hemograma.NaoHDL;

                // Atualiza o registro no contexto
                Contexto<Hemograma>().Update(hemogramaDb);

                // Log de atualização de hemograma
                if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
                {
                    Log logAtualizacaoHemograma = new Log()
                    {
                        DataAlteracao = DateTime.Now,
                        TabelaReferencia = "Hemograma",
                        Alteracao = $"Atualização do hemograma para consulta ID {hemograma.IdConsulta}",
                        IdUsuarioAlteracao = UsuarioLogado.Id
                    };

                    Contexto<Log>().Add(logAtualizacaoHemograma); // Adicionando log
                    SaveChanges();
                }

            }
            SaveChanges();

            DisplayMensagemSucesso();
            return Json(new { success = true });
        }


        [HttpPost]
        public async Task<JsonResult> SalvarAbaConsulta(Consulta abaConsulta)
        {
            var consultaBd = Contexto<Consulta>().Include(x => x.Prescricao).ThenInclude(x => x.Medicamentos).FirstOrDefault(x => x.Id == abaConsulta.Id);

            consultaBd.QueixaPrincipal = abaConsulta.QueixaPrincipal;
            consultaBd.HistoricoDoencaAtual = abaConsulta.HistoricoDoencaAtual;
            consultaBd.Orientacoes = abaConsulta.Orientacoes;
            consultaBd.HipoteseDiagnostica = abaConsulta.HipoteseDiagnostica;
            consultaBd.ExamesSolicitados = abaConsulta.ExamesSolicitados;
            consultaBd.RetornoConsulta = abaConsulta.RetornoConsulta;




            if (abaConsulta.Prescricao != null)
            {
                abaConsulta.Prescricao.IdConsulta = abaConsulta.Id;
                if (consultaBd.Prescricao == null || consultaBd.Prescricao.Id != abaConsulta.Prescricao.Id)
                {
                    Contexto<Prescricao>().Add(abaConsulta.Prescricao);

                }
                else
                {
                    foreach (var item in abaConsulta.Prescricao.Medicamentos)
                    {
                        if (!consultaBd.Prescricao.Medicamentos.Any(x => x.Id == item.Id))
                        {
                            item.IdPrescricao = consultaBd.Prescricao.Id;
                            Contexto<Medicamento>().Add(item);
                        }
                    }
                }

            }



            SaveChanges();
            consultaBd.IdPrescricao = abaConsulta.Prescricao.Id;
            SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> ExcluirMedicamentoPrescricao(int Id)
        {
            var MedicamentoBd = Contexto<Medicamento>().FirstOrDefault(x => x.Id == Id);
            if (MedicamentoBd != null)
                Contexto<Medicamento>().Remove(MedicamentoBd);
            //registrarLog

            SaveChanges();
            ModelState.Clear();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<JsonResult> ConcluirConsulta(int idConsulta)
        {
            var consulta = Contexto<Consulta>().FirstOrDefault(x => x.Id == idConsulta);

            if (consulta != null)
            {
                consulta.StatusConsulta = (int)StatusConsulta.Finalizada;
                consulta.DataConclusao = DateTime.Now;
            }

            SaveChanges();
            ModelState.Clear();

            return Json(new { success = true });
        }
    }
}
