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
            return View();
        }

        public async Task<ActionResult> Consultas()
        {
            return View();
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

            var paciente = Contexto<Paciente>()
                .Include(x => x.Antecedentes)
                .Include(x => x.Consultas)
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

            if (Id > 0)
                return View("Views\\Paciente\\editar2.cshtml", paciente);
            else
                return View("Views\\Paciente\\editar.cshtml", paciente);
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
            var pacienteBD = await Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefaultAsync(x => x.Id == paciente.Id);

            if (pacienteBD != null)
            {
                pacienteBD.Antecedentes?.Clear();
                pacienteBD = paciente;
            }
            else
                Contexto<Paciente>().Add(paciente);

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
            return View("Views\\Paciente\\editar2.cshtml", paciente);
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
        public JsonResult AdicionarArquivo(IFormFile file, int idPaciente)
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
                NomeArquivo = file.FileName.Split(".")[0],
                ArquivoData = fileData,
                IdTabelaReferencia = idPaciente.ToString(),
                NmTabelaReferencia = "Paciente",
                TipoArquivo = file.FileName.Split(".")[1],
                Ativo = true
            };

            Contexto<Anexo>().Add(anexo);
            SaveChanges();

            return Json(new { success = true, anexo });
        }

        [HttpPost]
        public JsonResult DesativarArquivo(int idAnexo)
        {
            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);
            anexo.Ativo = false;
            SaveChanges();

            return Json(new { success = true, anexo });
        }

        [HttpPost]
        public IActionResult BaixarArquivo(int idAnexo)
        {
            var anexo = Contexto<Anexo>().FirstOrDefault(x => x.IdAnexo == idAnexo);                                  
            

            return File(anexo.ArquivoData, "application/pdf", anexo.NomeArquivo);
        }
    }


}
