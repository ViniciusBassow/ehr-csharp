using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class ConsultaController : GlobalController
    {
        public ConsultaController(AppDbContext context) : base(context)
        {
        }

        //wwwroot\Calendar\fullcalendar\packages\core\main.js           -> CONTROLE CALENDARIUO
        //wwwroot\Calendar\fullcalendar\packages\core\main.css          -> ESTILOS
        public ActionResult Index()
        {
            List<Consulta> consultas = Contexto<Consulta>().ToList();
            

            return View("Views\\Consulta\\Index.cshtml", consultas);
        }
    }
}
