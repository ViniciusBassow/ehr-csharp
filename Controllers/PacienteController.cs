using Azure.Core;
using Azure;
using ehr_csharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Globalization;


namespace ehr_csharp.Controllers
{

    public class PacienteController : GlobalController
    {
        private readonly IMemoryCache _cache;
        private readonly UserManager<Usuario> _userManager;

        public PacienteController(AppDbContext context, UserManager<Usuario> userManager, IMemoryCache cache) : base(context, cache)
        {
            _cache = cache;
            _userManager = userManager;
        }

        [CustomAuthorize("Admin", "Agenda", "Medico")]
        public async Task<ActionResult> Index()
        {
            List<Paciente> pacientes = Contexto<Paciente>().Include(x => x.Consultas).ToList();
            return View(pacientes);
        }

        [CustomAuthorize("Admin", "Agenda", "Medico")]
        public async Task<ActionResult> Editar(int Id)
        {
            // Log de início de edição de paciente
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                // Criando o log de início de edição do paciente
                Log logInicioEdicaoCache = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Paciente",
                    Alteracao = $"Início da edição do paciente com ID {Id}",
                    IdUsuarioAlteracao = usuarioLogado.Id
                };

                Contexto<Log>().Add(logInicioEdicaoCache);
                SaveChanges();
            }

            var paciente = Contexto<Paciente>()
                            .Include(x => x.Antecedentes)
                            .Include(x => x.Consultas)
                                .ThenInclude(x => x.Medico)
                                    .ThenInclude(x => x.Usuario)
                            .Include(x => x.Consultas)
                                .ThenInclude(x => x.Medico)
                                    .ThenInclude(x => x.Especialidade)
                            .Include(x => x.Consultas)
                                .ThenInclude(x => x.Prescricao)
                                .ThenInclude(x => x.Medicamentos)
                            .FirstOrDefault(x => x.Id == Id);

            if (paciente == null)
                paciente = new Paciente();
            if (paciente.Antecedentes == null)
                paciente.Antecedentes = new List<Antecedente>();

            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogadoFinal))
                usuarioLogadoFinal.Role = _userManager.GetRolesAsync(usuarioLogadoFinal).Result.FirstOrDefault();

            // Criando o log de início de edição do paciente (fora do bloco if)
            Log logInicioEdicaoFinal = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Paciente",
                Alteracao = $"Início da edição do paciente com ID {Id}",
                IdUsuarioAlteracao = usuarioLogadoFinal.Id
            };

            Contexto<Log>().Add(logInicioEdicaoFinal); // Adicionando log
            SaveChanges();

            if (usuarioLogadoFinal.Role == "Admin")
                paciente.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Paciente" && x.IdTabelaReferencia == Id.ToString()).ToList();
            else
                paciente.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Paciente" && x.IdTabelaReferencia == Id.ToString() && x.Ativo).ToList();

            if (paciente.Consultas != null && paciente.Consultas.Count > 0)
            {
                var maiorIdConsulta = paciente.Consultas.OrderByDescending(x => x.Data).First().Id;

                paciente.ultimaConsultaHemograma = await Contexto<Hemograma>().FirstOrDefaultAsync(x => x.IdConsulta == maiorIdConsulta);
                paciente.ultimaConsulta = paciente.Consultas.OrderByDescending(x => x.Data).FirstOrDefault(x => x.StatusConsulta == (int)StatusConsulta.EmAndamento);


                foreach(var item in paciente.Consultas)
                {
                    item.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Consulta" && x.IdTabelaReferencia == item.Id.ToString()).ToList();
                }
                
            }

            if (paciente.ultimaConsultaHemograma == null)
                paciente.ultimaConsultaHemograma = new Hemograma();
            
            if(paciente.ultimaConsulta != null)
            {
                paciente.ultimaConsulta.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Consulta" && x.IdTabelaReferencia == paciente.ultimaConsulta.Id.ToString()).ToList();
            }

            if (Id > 0)
                return View("Views\\Paciente\\editar2.cshtml", paciente);
            else
                return View("Views\\Paciente\\editar2.cshtml", paciente);
        }

        [CustomAuthorize("Admin", "Medico")]
        [HttpPost]
        public async Task<ActionResult> Salvar(Paciente paciente)
        {
            ModelState.Clear();
            ValidarCamposPaciente(paciente);

            if (!ModelState.IsValid)
            {
                return View("Views\\Paciente\\editar2.cshtml", new Paciente() { Antecedentes = new List<Antecedente>() });
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();


            if (paciente.File != null)
                paciente.ImagemBase64 = Usuario.Helper.ConverterImagemEmString(paciente.File);


            var pacienteBD = await Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefaultAsync(x => x.Id == paciente.Id);

            if (pacienteBD != null)
            {
                pacienteBD.Antecedentes?.Clear();
                pacienteBD = paciente;
            }
            else
            {
                paciente.DataCadastro = DateTime.Now;
                Contexto<Paciente>().Add(paciente);

                

                Usuario usuario = new Usuario()
                {
                    Email = paciente.Email,
                    Name = paciente.NomeCompleto,
                    UserName = paciente.Email,
                    Role = "Paciente",
                    Password = ConsultarConfig("SenhaPadraoPaciente").ToString().Replace("_TagSenha_", paciente.Cpf.Replace(".", "").Replace("-", "")),
                    PasswordHash = Usuario.Helper.HashPassword((ConsultarConfig("SenhaPadraoPaciente").ToString().Replace("_TagSenha_", paciente.Cpf.Replace(".","").Replace("-", "")))),
                    CreatedAt = DateTime.Now
                };

                if (paciente.File != null)
                    usuario.ImageByteStr = Usuario.Helper.ConverterImagemEmString(paciente.File);

                var result = await _userManager.CreateAsync(usuario, usuario.Password);
                
                if (result.Succeeded)
                {
                    var addToRoleResult = await _userManager.AddToRoleAsync(usuario, usuario.Role);

                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            //errors.Add(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        //errors.Add(string.Empty, error.Description);
                    }
                }
            }
            // Log de criação ou atualização de paciente
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logSalvarPaciente = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Paciente",
                    Alteracao = pacienteBD == null ? "Criação de novo paciente" : $"Atualização do paciente com ID {paciente.Id}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logSalvarPaciente); // Adicionando log
                SaveChanges();
            }


            int contadorHeader = 0;
            while (true)
            {
                string descricao = Request.Form[$"Antecedentes{contadorHeader}"];
                if (string.IsNullOrEmpty(descricao))
                    break;


                var Antecendete = new Antecedente()
                {
                    Descricao = descricao,
                    PacienteId = paciente.Id
                };

                Contexto<Antecedente>().Add(Antecendete);
                paciente.Antecedentes.Add(Antecendete);
                contadorHeader++;
            }

            SaveChanges();

            DisplayMensagemSucesso();

            return RedirectToAction("Index", "Paciente");
            //return View("Views\\Paciente\\editar2.cshtml", paciente);
        }

        public void ValidarCamposPaciente(Paciente paciente)
        {
            if (string.IsNullOrEmpty(paciente.NomeCompleto))
                ModelState.AddModelError("Nome Completo", "O campo é obrigatório");
            if (paciente.DataNascimento == new DateOnly())
                ModelState.AddModelError("Data de Nascimento", "O campoo é obrigatório");
            if (string.IsNullOrEmpty(paciente.Sexo))
                ModelState.AddModelError("Sexo", "O campo é obrigatório");

            if (string.IsNullOrEmpty(paciente.Cpf))
                ModelState.AddModelError("CPF", "O campo é obrigatório");
            if (string.IsNullOrEmpty(paciente.Rg))
                ModelState.AddModelError("RG", "O campo é obrigatório");
            if (string.IsNullOrEmpty(paciente.Profissao))
                ModelState.AddModelError("Profissão", "O campo é obrigatório");
            if (string.IsNullOrEmpty(paciente.EstadoCivil))
                ModelState.AddModelError("Estado Civil", "O campo é obrigatório");

            if (string.IsNullOrEmpty(paciente.Cep))
                ModelState.AddModelError("CEP", "O campo é obrigatório");
            if (string.IsNullOrEmpty(paciente.Endereco))
                ModelState.AddModelError("Endereço", "O campo Endereço é obrigatório");

        }

        [HttpPost]
        public JsonResult AdicionarArquivo(IFormFile file, int idPaciente, string nmArquivo)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido.");

            // Converte o IFormFile para um array de bytes
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                fileData = memoryStream.ToArray();
            }

            Anexo anexo = new Anexo()
            {
                NomeArquivo = nmArquivo,
                ArquivoData = fileData,
                IdTabelaReferencia = idPaciente.ToString(),
                NmTabelaReferencia = "Paciente",
                TipoArquivo = file.FileName.Split(".")[1],
                Ativo = true
            };

            Contexto<Anexo>().Add(anexo);
            SaveChanges();

            // Log de adição de arquivo
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logAdicaoArquivo = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Paciente",
                    Alteracao = $"Adição de arquivo '{nmArquivo}' ao paciente com ID {idPaciente}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logAdicaoArquivo); // Adicionando log
                SaveChanges();
            }


            return Json(new { success = true, anexo });
        }

        [CustomAuthorize("Admin", "Medico")]
        [HttpPost]
        public JsonResult DesativarArquivo(int idAnexo)
        {
            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);

            // Log de início de desativação de arquivo
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logDesativacaoArquivo = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Anexo",
                    Alteracao = $"Início de desativação do arquivo com ID {idAnexo}",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logDesativacaoArquivo); // Adicionando log
                SaveChanges();
            }


            anexo.Ativo = false;
            SaveChanges();

            return Json(new { success = true, anexo });
        }

        [HttpPost]
        public IActionResult BaixarArquivo(int idAnexo)
        {

            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);

            // Log de download de arquivo
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                Log logDownloadArquivo = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Anexo",
                    Alteracao = $"Download do arquivo com ID {idAnexo} solicitado",
                    IdUsuarioAlteracao = UsuarioLogado.Id
                };

                Contexto<Log>().Add(logDownloadArquivo); // Adicionando log
                SaveChanges();
            }


            var mimeType = anexo.TipoArquivo switch
            {
                "pdf" => "application/pdf",
                "jpeg" or "jpg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "doc" => "application/msword",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream" // Tipo genérico
            };

            return File(anexo.ArquivoData, mimeType, anexo.NomeArquivo);
        }

        public async Task<ActionResult> FiltrarPacientes(bool somenteComConsulta)
        {

            List<Paciente> pacientes = Contexto<Paciente>().Include(x => x.Consultas).ToList();
            var retorno = new List<Paciente>();

            if (somenteComConsulta)
                retorno.AddRange(pacientes.Where(x => x.Consultas.Any(y => y.StatusConsulta == (int)StatusConsulta.EmAndamento)).ToList());
            else
                retorno.AddRange(pacientes);

            return PartialView("~/Views/Paciente/_ListaPaciente.cshtml", retorno);

        }




        [CustomAuthorize("Paciente")]
        public async Task<ActionResult> Perfil()
        {
            var idPaciente = 0;
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                idPaciente = Contexto<Paciente>().FirstOrDefault(x => x.Email == usuarioLogado.Email).Id;
            }

            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                         .ThenInclude(x => x.Medico)
                                         .ThenInclude(x => x.Especialidade)
                                         .Include(x => x.Consultas)
                                         .ThenInclude(x => x.Medico)
                                         .ThenInclude(x => x.Usuario)
                                         .Include(x => x.Consultas)
                                         .ThenInclude(x => x.Prescricao)
                                         .ThenInclude(x => x.Medicamentos)
                                         .FirstOrDefault(x => x.Id == idPaciente);

            return View(paciente);
        }
        [CustomAuthorize("Paciente")]
        public async Task<ActionResult> Consultas()
        {
            var idPaciente = 0;
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                idPaciente = Contexto<Paciente>().FirstOrDefault(x => x.Email == usuarioLogado.Email).Id;
            }
            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                        .ThenInclude(x => x.Medico)
                                        .ThenInclude(x => x.Especialidade)
                                        .Include(x => x.Consultas)
                                        .ThenInclude(x => x.Medico)
                                        .ThenInclude(x => x.Usuario)
                                        .FirstOrDefault(x => x.Id == idPaciente);


            foreach (var item in paciente.Consultas)
            {
                item.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Consulta" && x.IdTabelaReferencia == item.Id.ToString()).ToList();
            }

            return View(paciente);
        }
        [CustomAuthorize("Paciente")]
        public async Task<ActionResult> RegistrosMedicos()
        {
            var idPaciente = 0;
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                idPaciente = Contexto<Paciente>().FirstOrDefault(x => x.Email == usuarioLogado.Email).Id;
            }

            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                                    .ThenInclude(x => x.Prescricao)
                                                    .ThenInclude(x => x.Medicamentos)
                                                    .Include(x => x.Consultas)
                                                    .ThenInclude(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                    .FirstOrDefault(x => x.Id == idPaciente);

            return View();
        }
        [CustomAuthorize("Paciente")]
        public async Task<ActionResult> Medicacoes()
        {
            var idPaciente = 0;
            if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
            {
                idPaciente = Contexto<Paciente>().FirstOrDefault(x => x.Email == usuarioLogado.Email).Id;
            }

            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                                    .ThenInclude(x => x.Prescricao)
                                                    .ThenInclude(x => x.Medicamentos)
                                                    .Include(x => x.Consultas)
                                                    .ThenInclude(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                    .FirstOrDefault(x => x.Id == idPaciente);

            return View(paciente);
        }

        [HttpPost]
        public JsonResult AnexarArquivo(IFormFile file, int idConsulta, string tipoArquivo, string nmArquivo)
        {

            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inválido.");

            // Converte o IFormFile para um array de bytes
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                fileData = memoryStream.ToArray();
            }

            var anexoBd = Contexto<Anexo>().FirstOrDefault(x => x.IdTabelaReferencia == idConsulta.ToString() && x.NmTabelaReferencia == "Consulta" && x.TipoDocumento == tipoArquivo);

            anexoBd.ArquivoData = fileData;
            anexoBd.NomeArquivo = nmArquivo;
            anexoBd.Status = "Em análise";



            SaveChanges();


            return Json(new { success = true });
        }
    }


}
