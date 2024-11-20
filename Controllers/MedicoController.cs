using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ehr_csharp.Models;
using SQLApp.Data;

namespace ehr_csharp.Controllers
{
    [Route("medico")] 
    public class MedicoController : Controller
    {
        private readonly AppDbContext _context;

        public MedicoController(AppDbContext context)
        {
            _context = context;
        }

        [Route("dash")]
        public IActionResult Dash()
        {
            var medico = _context.Medicos;
            return View(medico); 
        }

        [Route("agendamento")]
        public IActionResult Agendamento()
        {
            return View();
        }
    }
}
