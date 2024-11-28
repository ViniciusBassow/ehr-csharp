using Microsoft.AspNetCore.Mvc;
using ehr_csharp.Models; 
using System.Collections.Generic;  

namespace ehr_csharp.Controllers
{
    [CustomAuthorize("Medico")]
    [Route("medico")]
    public class MedicoController : Controller
    {
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
