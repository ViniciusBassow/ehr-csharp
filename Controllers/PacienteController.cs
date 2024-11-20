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

        public PacienteController(AppDbContext context, UserManager<Usuario> userManager, IMemoryCache cache) : base(context)
        {
            _cache = cache;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            List<Paciente> pacientes = Contexto<Paciente>().Include(x => x.Consultas).ToList();
            return View(pacientes);
        }


        public async Task<ActionResult> Perfil()
        {
            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                         .ThenInclude(x => x.Medico)
                                         .ThenInclude(x => x.Especialidade)
                                         .Include(x => x.Consultas)
                                         .ThenInclude(x => x.Medico)
                                         .ThenInclude(x => x.Usuario)
                                         .FirstOrDefault(x => x.Id == 3);

            return View(paciente);
        }

        public async Task<ActionResult> Consultas()
        {
            Paciente paciente = Contexto<Paciente>().Include(x => x.Consultas)
                                        .ThenInclude(x => x.Medico)
                                        .ThenInclude(x => x.Especialidade)
                                        .Include(x => x.Consultas)
                                        .ThenInclude(x => x.Medico)
                                        .ThenInclude(x => x.Usuario)
                                        .FirstOrDefault(x => x.Id == 3);
            return View(paciente);
        }


        public async Task<ActionResult> RegistrosMedicos()
        {
            return View();
        }

        public async Task<ActionResult> Medicacoes()
        {
            return View();
        }

        public async Task<ActionResult> Editar(int Id)
        {
            // Log de início de edição de paciente
            Log logInicioEdicao = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Paciente",
                Alteracao = $"Início da edição do paciente com ID {Id}"
            };
            //Contexto<Log>().Add(logInicioEdicao); // Adicionando log
            SaveChanges();

            var paciente = Contexto<Paciente>()
                            .Include(x => x.Antecedentes)
                            .Include(x => x.Consultas)
                                .ThenInclude(x => x.Medico)
                                    .ThenInclude(x => x.Usuario)
                            .Include(x => x.Consultas)
                                .ThenInclude(x => x.Prescricao)
                                .ThenInclude(x => x.Medicamentos)
                            .FirstOrDefault(x => x.Id == Id);

            if (paciente == null)
                paciente = new Paciente();
            if (paciente.Antecedentes == null)
                paciente.Antecedentes = new List<Antecedente>();

            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
                UsuarioLogado.Role = _userManager.GetRolesAsync(UsuarioLogado).Result.FirstOrDefault();

            if (UsuarioLogado.Role == "Admin")
                paciente.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Paciente" && x.IdTabelaReferencia == Id.ToString()).ToList();
            else
                paciente.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Paciente" && x.IdTabelaReferencia == Id.ToString() && x.Ativo).ToList();

            if (paciente.Consultas != null && paciente.Consultas.Count > 0)
            {
                var maiorIdConsulta = paciente.Consultas.OrderByDescending(x => x.Data).First().Id;

                paciente.ultimaConsultaHemograma = await Contexto<Hemograma>().FirstOrDefaultAsync(x => x.IdConsulta == maiorIdConsulta);
                paciente.ultimaConsulta = paciente.Consultas.OrderByDescending(x => x.Data).First();


            }

            if (paciente.ultimaConsultaHemograma == null)
                paciente.ultimaConsultaHemograma = new Hemograma();

            if (Id > 0)
                return View("Views\\Paciente\\editar2.cshtml", paciente);
            else
                return View("Views\\Paciente\\editar2.cshtml", paciente);
        }

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
            }
            // Log de criação ou atualização de paciente
            Log logSalvarPaciente = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Paciente",
                Alteracao = pacienteBD == null ? "Criação de novo paciente" : $"Atualização do paciente com ID {paciente.Id}"
            };
            //Contexto<Log>().Add(logSalvarPaciente); // Adicionando log

            SaveChanges();

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
            Log logAdicaoArquivo = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Paciente",
                Alteracao = $"Adição de arquivo '{nmArquivo}' ao paciente com ID {idPaciente}"
            };
            //Contexto<Log>().Add(logAdicaoArquivo); // Adicionando log

            return Json(new { success = true, anexo });
        }

        [HttpPost]
        public JsonResult DesativarArquivo(int idAnexo)
        {
            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);

            // Log de início de desativação de arquivo
            Log logDesativacaoArquivo = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Anexo",
                Alteracao = $"Início de desativação do arquivo com ID {idAnexo}"
            };
            //Contexto<Log>().Add(logDesativacaoArquivo); // Adicionando log

            anexo.Ativo = false;
            SaveChanges();

            return Json(new { success = true, anexo });
        }

        [HttpPost]
        public IActionResult BaixarArquivo(int idAnexo)
        {

            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);

            // Log de download de arquivo
            Log logDownloadArquivo = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Anexo",
                Alteracao = $"Download do arquivo com ID {idAnexo} solicitado"
            };
            //Contexto<Log>().Add(logDownloadArquivo); // Adicionando log

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
    }


}
