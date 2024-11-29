using Microsoft.AspNetCore.Mvc;
using ehr_csharp.Models; 
using System.Collections.Generic;
using SQLApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace ehr_csharp.Controllers
{
    [CustomAuthorize("Medico")]
    [Route("medico")]
    public class MedicoController : GlobalController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IMemoryCache _cache;

        public MedicoController(AppDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Usuario> signInManager, IMemoryCache cache) : base(context, cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _cache = cache;
        }

        private static List<Agendamento> Agendamentos = new List<Agendamento>();

   
        [Route("dash")]
        public IActionResult Dash()
        {
            return View("Dash");
        }

        [Route("agendamento")]
        public IActionResult Agendamento()
        {
            ViewBag.Agendamentos = Agendamentos;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("agendamento")]
        public IActionResult Agendamento(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {

                Agendamentos.Add(agendamento);

                return RedirectToAction(nameof(Agendamento));
            }

            return View(agendamento);
        }
        
        [Route("config")]
        public IActionResult config()
        {
            return View("config"); 
        }
    }
}
