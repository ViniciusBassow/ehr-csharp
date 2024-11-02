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
using Serilog; // Adicione esta linha para usar o Serilog

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
            Log.Information("Acessando a lista de pacientes."); // Log de informa��o ao acessar a lista de pacientes
            List<Paciente> pacientes = Contexto<Paciente>().ToList();
            return View(pacientes);
        }

        public async Task<ActionResult> Editar(int Id)
        {
            Log.Information("Editando paciente com ID: {Id}", Id); // Log de informa��o ao editar um paciente

            var paciente = Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefault(x => x.Id == Id);
            if (paciente == null)
            {
                Log.Warning("Paciente com ID: {Id} n�o encontrado.", Id); // Log de aviso se o paciente n�o for encontrado
                paciente = new Paciente();
            }
            if (paciente.Antecedentes == null)
                paciente.Antecedentes = new List<Antecedente>();

            return View(paciente);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Paciente paciente)
        {
            Log.Information("Salvando paciente: {Paciente}", paciente); // Log de informa��o ao salvar um paciente

            ValidarCamposPaciente(paciente);

            if (!ModelState.IsValid)
            {
                Log.Warning("ModelState inv�lido para paciente: {Paciente}", paciente); // Log de aviso se o ModelState n�o for v�lido
                return View("Views\\Usuario\\editar.cshtml", new Usuario());
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();
            var pacienteBD = await Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefaultAsync(x => x.Id == paciente.Id);

            if (pacienteBD != null)
            {
                pacienteBD.Antecedentes?.Clear();
                pacienteBD = paciente;
                Log.Information("Atualizando paciente com ID: {Id}", paciente.Id); // Log de informa��o ao atualizar um paciente
            }
            else
            {
                Log.Information("Adicionando novo paciente: {Paciente}", paciente); // Log de informa��o ao adicionar um novo paciente
                Contexto<Paciente>().Add(paciente);
            }

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
            Log.Information("Paciente salvo com sucesso: {Paciente}", paciente); // Log de informa��o ap�s salvar o paciente
            return View("Views\\Paciente\\editar.cshtml", paciente);
        }

        public void ValidarCamposPaciente(Paciente paciente)
        {
            if (string.IsNullOrEmpty(paciente.NomeCompleto))
                ModelState.AddModelError("Nome Completo", "O campo � obrigat�rio");
            if (paciente.DataNascimento == new DateTime())
                ModelState.AddModelError("Data de Nascimento", "O campo � obrigat�rio");
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
    }
}
