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
        public PDFController(AppDbContext context, IMemoryCache cache, UserManager<Usuario> userManager) : base(context)
        {
            _cache = cache;
            _userManager = userManager;
        }

        public IActionResult GerarAtestado(int idConsulta)
        {
            idConsulta = 8;
            var consulta = Contexto<Consulta>().Include(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                .Include(x => x.Medico)
                                                    .ThenInclude(x => x.Especialidade)
                                                    .Include(x => x.Paciente)
                                                .FirstOrDefault(x => x.Id == idConsulta);            

            return new ViewAsPdf("~/Views/Template/Atestado.cshtml", consulta)
            {
                FileName = "Relatorio.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10)
            };
        }

        public IActionResult teste(int idConsulta)
        {
            idConsulta = 8;
            var consulta = Contexto<Consulta>().Include(x => x.Medico)
                                                    .ThenInclude(x => x.Usuario)
                                                .Include(x => x.Medico)
                                                    .ThenInclude(x => x.Especialidade)
                                                    .Include(x => x.Paciente)
                                                .FirstOrDefault(x => x.Id == idConsulta);

            return View("~/Views/Template/Atestado.cshtml", consulta);            
        }
    }
}
