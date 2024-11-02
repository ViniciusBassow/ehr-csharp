using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System.Diagnostics;
using Serilog; // Adicione esta linha para usar o Serilog

namespace ehr_csharp.Controllers
{
    public class ConsultaController : GlobalController
    {
        private readonly IMemoryCache _cache;

        public ConsultaController(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public ActionResult Index()
        {
            List<Consulta> consultas = new List<Consulta>();
            if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
            {
                if (UsuarioLogado.Medico != null)
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).Where(x => x.IdMedico == UsuarioLogado.Medico.Id).ToList());
                else if (UsuarioLogado.Role == "Admin")
                    consultas.AddRange(Contexto<Consulta>().Include(x => x.Paciente).ToList());
            }

            return View("Views\\Consulta\\Index.cshtml", consultas);
        }

        // Método para salvar uma nova consulta (Insert)
        [HttpPost]
        public async Task<ActionResult> Salvar(Consulta consulta)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Tentativa de salvar consulta inválida: {@Consulta}", consulta); // Log de aviso se o modelo não for válido
                return View("Views\\Usuario\\editar.cshtml", new Consulta());
            }

            try
            {
                Contexto<Consulta>().Add(consulta);
                SaveChanges();
                Log.Information("Consulta criada: {@Consulta}", consulta); // Log de inserção ao salvar
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao salvar consulta: {@Consulta}", consulta); // Log de erro em caso de falha
                return View("Views\\Usuario\\editar.cshtml", new Consulta());
            }

            return View("Views\\Consulta\\Index.cshtml", consulta);
        }

        // Método para atualizar uma consulta (Update)
        [HttpPost]
        public async Task<ActionResult> Atualizar(Consulta consulta)
        {
            if (!ModelState.IsValid)
            {
                Log.Warning("Tentativa de atualizar consulta inválida: {@Consulta}", consulta); // Log de aviso se o modelo não for válido
                return View("Views\\Usuario\\editar.cshtml", consulta);
            }

            try
            {
                Contexto<Consulta>().Update(consulta);
                SaveChanges();
                Log.Information("Consulta atualizada: {@Consulta}", consulta); // Log de atualização
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao atualizar consulta: {@Consulta}", consulta); // Log de erro em caso de falha
                return View("Views\\Usuario\\editar.cshtml", consulta);
            }

            return View("Views\\Consulta\\Index.cshtml", consulta);
        }

        // Método para excluir uma consulta (Delete)
        [HttpPost]
        public async Task<ActionResult> Excluir(int id)
        {
            var consulta = await Contexto<Consulta>().FindAsync(id);
            if (consulta == null)
            {
                Log.Warning("Tentativa de excluir consulta que não existe: {ID}", id); // Log de aviso se a consulta não for encontrada
                return NotFound();
            }

            try
            {
                Contexto<Consulta>().Remove(consulta);
                SaveChanges();
                Log.Information("Consulta excluída: {@Consulta}", consulta); // Log de exclusão
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erro ao excluir consulta: {@Consulta}", consulta); // Log de erro em caso de falha
                return View("Views\\Usuario\\editar.cshtml", consulta);
            }

            return View("Views\\Consulta\\Index.cshtml");
        }
    }
}
