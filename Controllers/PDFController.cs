using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Rotativa.AspNetCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class PDFController : GlobalController
    {
        private readonly IMemoryCache _cache;
        private readonly UserManager<Usuario> _userManager;
        private readonly AppDbContext dbContext;
        public PDFController(AppDbContext context, IMemoryCache cache, UserManager<Usuario> userManager) : base(context, cache)
        {
            _cache = cache;
            _userManager = userManager;
            dbContext = context;
        }

        public IActionResult GerarComprovanteComparacimento(int idConsulta)
        {            
            var consulta = Contexto<Consulta>().Include(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                .Include(x => x.Medico)
                                                    .ThenInclude(x => x.Especialidade)
                                                    .Include(x => x.Paciente)
                                                .FirstOrDefault(x => x.Id == idConsulta);

            consulta.preencherCamposConfigTemplate(dbContext, _cache);


            RegistrarLog($"Gerou comprovante de Comparecimento da consulta {idConsulta}", "Consulta");

            return new ViewAsPdf("~/Views/Template/ComprovanteComparecimento.cshtml", consulta)
            {
                FileName = $"ComprovanteComparecimento{consulta.Paciente.NomeCompleto}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };
        }

        public IActionResult GerarAtestado(int idConsulta)
        {
            var consulta = Contexto<Consulta>().Include(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                .Include(x => x.Medico)
                                                    .ThenInclude(x => x.Especialidade)
                                                    .Include(x => x.Paciente)
                                                .FirstOrDefault(x => x.Id == idConsulta);

            consulta.preencherCamposConfigTemplate(dbContext, _cache);


            RegistrarLog($"Gerou atestado médico da consulta {idConsulta}", "Consulta");

            return new ViewAsPdf("~/Views/Template/Atestado.cshtml", consulta)
            {
                FileName = $"ComprovanteComparecimento{consulta.Paciente.NomeCompleto}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };
        }

        public IActionResult GerarPrescricaoMedica(int idConsulta)
        {
            var consulta = Contexto<Consulta>().Include(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                .Include(x => x.Medico)
                                                    .ThenInclude(x => x.Especialidade)
                                                    .Include(x => x.Paciente)
                                                     .Include(x => x.Prescricao)
                                                     .ThenInclude(x => x.Medicamentos)
                                                .FirstOrDefault(x => x.Id == idConsulta);

            consulta.preencherCamposConfigTemplate(dbContext, _cache);


            RegistrarLog($"Gerou atestado médico da consulta {idConsulta}", "Consulta");

            return new ViewAsPdf("~/Views/Template/PrescricaoMedica.cshtml", consulta)
            {
                FileName = $"ComprovanteComparecimento{consulta.Paciente.NomeCompleto}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };
        }

        public IActionResult Teste()
        {

            EnviarEmail("vinifbasso4@gmail.com", "teste", "OLÁ MUNDO");
            return View();
        }
    }
}
