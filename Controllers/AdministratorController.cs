using ehr_csharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;

namespace ehr_csharp.Controllers
{
    public class AdministratorController : GlobalController
    {

        public AdministratorController(AppDbContext context) : base(context)
        {
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> Usuarios()
        {
            return View();
        }

        public async Task<ActionResult> Calendario()
        {
            return View();
        }


        public async Task<ActionResult> Relatorios()
        {
            return View();
        }

        public async Task<ActionResult> Configuracoes()
        {
            return View();
        }

    }
}
