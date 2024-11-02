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
            Log.Information("Acessando a lista de pacientes."); // Log de informação ao acessar a lista de pacientes
            List<Paciente> pacientes = Contexto<Paciente>().ToList();
            return View(pacientes);
        }

        public async Task<ActionResult> Editar(int Id)
        {
            Log.Information("Editando paciente com ID: {Id}", Id); // Log de informação ao editar um paciente

            var paciente = Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefault(x => x.Id == Id);
            if (paciente == null)
            {
                Log.Warning("Paciente com ID: {Id} não encontrado.", Id); // Log de aviso se o paciente não for encontrado
                paciente = new Paciente();
            }
            if (paciente.Antecedentes == null)
                paciente.Antecedentes = new List<Antecedente>();

            return View(paciente);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Paciente paciente)
        {
            Log.Information("Salvando paciente: {Paciente}", paciente); // Log de informação ao salvar um paciente

            ValidarCamposPaciente(paciente);

            if (!ModelState.IsValid)
            {
                Log.Warning("ModelState inválido para paciente: {Paciente}", paciente); // Log de aviso se o ModelState não for válido
                return View("Views\\Usuario\\editar.cshtml", new Usuario());
            }

            Dictionary<string, string> errors = new Dictionary<string, string>();
            var pacienteBD = await Contexto<Paciente>().Include(x => x.Antecedentes).FirstOrDefaultAsync(x => x.Id == paciente.Id);

            if (pacienteBD != null)
            {
                pacienteBD.Antecedentes?.Clear();
                pacienteBD = paciente;
                Log.Information("Atualizando paciente com ID: {Id}", paciente.Id); // Log de informação ao atualizar um paciente
            }
            else
            {
                Log.Information("Adicionando novo paciente: {Paciente}", paciente); // Log de informação ao adicionar um novo paciente
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
            Log.Information("Paciente salvo com sucesso: {Paciente}", paciente); // Log de informação após salvar o paciente
            return View("Views\\Paciente\\editar.cshtml", paciente);
        }

        public void ValidarCamposPaciente(Paciente paciente)
        {
            if (string.IsNullOrEmpty(paciente.NomeCompleto))
                ModelState.AddModelError("Nome Completo", "O campo é obrigatório");
            if (paciente.DataNascimento == new DateTime())
                ModelState.AddModelError("Data de Nascimento", "O campo é obrigatório");
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
    }
}
