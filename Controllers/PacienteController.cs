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
        private readonly UserManager<Usuario> _userManager;

        public PacienteController(AppDbContext context, UserManager<Usuario> userManager) : base(context)
        {
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {
            List<Paciente> pacientes = Contexto<Paciente>().Include(x => x.Consultas).ToList();
            return View(pacientes);
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

            paciente.Anexos = Contexto<Anexo>().Where(x => x.NmTabelaReferencia == "Paciente" && x.IdTabelaReferencia == Id.ToString()).ToList();

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
                ModelState.AddModelError("Nome Completo", "O campo � obrigat�rio");
            if (paciente.DataNascimento == new DateOnly())
                ModelState.AddModelError("Data de Nascimento", "O campoo � obrigat�rio");
            if (string.IsNullOrEmpty(paciente.Sexo))
                ModelState.AddModelError("Sexo", "O campo � obrigat�rio");

            if (string.IsNullOrEmpty(paciente.Cpf))
                ModelState.AddModelError("CPF", "O campo � obrigat�rio");
            if (string.IsNullOrEmpty(paciente.Rg))
                ModelState.AddModelError("RG", "O campo � obrigat�rio");
            if (string.IsNullOrEmpty(paciente.Profissao))
                ModelState.AddModelError("Profiss�o", "O campo � obrigat�rio");
            if (string.IsNullOrEmpty(paciente.EstadoCivil))
                ModelState.AddModelError("Estado Civil", "O campo � obrigat�rio");

            if (string.IsNullOrEmpty(paciente.Cep))
                ModelState.AddModelError("CEP", "O campo � obrigat�rio");
            if (string.IsNullOrEmpty(paciente.Endereco))
                ModelState.AddModelError("Endere�o", "O campo Endere�o � obrigat�rio");
        }



        [HttpPost]
        public JsonResult UploadFile(IFormFile file, int idPaciente, string tipoArquivo)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Arquivo inv�lido.");

            // Converte o IFormFile para um array de bytes
            byte[] fileData;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                fileData = memoryStream.ToArray();
            }

            Anexo anexo = new Anexo()
            {
                NomeArquivo = file.FileName,
                ArquivoData = fileData.ToString(),
                IdTabelaReferencia = idPaciente.ToString(),
                NmTabelaReferencia = "Paciente",
                TipoArquivo = tipoArquivo
            };

            // Retornar o resultado como JSON para a View
            return Json(new { success = true });
        }
    }


}
