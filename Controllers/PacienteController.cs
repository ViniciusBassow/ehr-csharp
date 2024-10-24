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
            List<Paciente> pacientes = Contexto<Paciente>().ToList();
            return View(pacientes);
        }

        public async Task<ActionResult> Editar(int Id)
        {

            var paciente = Contexto<Paciente>().FirstOrDefault(x => x.Id == Id);
            if (paciente == null)
                paciente = new Paciente();
            

            return View(paciente);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Paciente paciente)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            var pacienteBD = await Contexto<Paciente>().FirstOrDefaultAsync(x => x.Id == paciente.Id);


            ValidarCamposPaciente(paciente);

            if (!ModelState.IsValid)
            {
                return View("Views\\Usuario\\editar.cshtml", new Usuario());
            }

            if (pacienteBD != null)
            {
               

            }
            else
            {
                Contexto<Paciente>().Add(paciente);
            }
            SaveChanges();            

            return View("Views\\Paciente\\editar.cshtml", paciente);
        }

        public void ValidarCamposPaciente(Paciente paciente)
        {
            if (string.IsNullOrEmpty(paciente.NomeCompleto))
                ModelState.AddModelError("Nome Completo", "O campo é obrigatório");
            if (paciente.DataNascimento == new DateTime())
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

    }


}
