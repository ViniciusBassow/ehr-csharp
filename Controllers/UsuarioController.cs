using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;
using System.Diagnostics;

namespace ehr_csharp.Controllers
{
    public class UsuarioController : GlobalController
    {
        public UsuarioController(AppDbContext context) : base(context)
        {
        }

        public ActionResult Index()
        {
            List<Usuario> usuarios = Contexto<Usuario>().ToList();

            return View(usuarios);
        }

        public ActionResult Editar(string Id)
        {
            var usuario = new Usuario();
            usuario = Contexto<Usuario>().FirstOrDefault(x => x.Id == Id);

            return View(usuario);
        }

        public ActionResult Salvar(Usuario usuario)
        {
            if (int.Parse(usuario.Id) > 0) { 
                var usuarioBD = Contexto<Usuario>().FirstOrDefault(x => x.Id == usuario.Id);
                usuarioBD.UserName = usuario.UserName;
                usuarioBD.Email = usuario.Email;
                usuarioBD.Email = usuario.Email;


            }

            else
                Contexto<Usuario>().Add(usuario);

            return View(usuario);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
