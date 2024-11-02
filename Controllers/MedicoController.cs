using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ehr_csharp.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ehr_csharp.Controllers
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Medico> Medicos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }
        // Adicione outros DbSet conforme necessário
    }

    public class MedicoController : Controller
    {
        private readonly AppDbContext _context;

        public MedicoController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Medico
        public async Task<IActionResult> Index()
        {
            Log.Information("Acessando a lista de médicos.");
            var medicos = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidade)
                .ToListAsync();
            return View(medicos);
        }

        // GET: Medico/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Log.Information("Acessando detalhes do médico com ID: {Id}", id);
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidade)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medico == null)
            {
                Log.Warning("Médico com ID: {Id} não encontrado.", id);
                return NotFound();
            }

            return View(medico);
        }

        // GET: Medico/Create
        public IActionResult Create()
        {
            Log.Information("Acessando página de criação de médico.");
            return View();
        }

        // POST: Medico/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CRM,IdUsuario,IdEspecialidade")] Medico medico)
        {
            Log.Information("Tentando salvar um novo médico: {Medico}", medico);
            if (ModelState.IsValid)
            {
                _context.Add(medico);
                await _context.SaveChangesAsync();
                Log.Information("Médico criado com sucesso: {Medico}", medico);
                return RedirectToAction(nameof(Index));
            }
            Log.Warning("ModelState inválido para médico: {Medico}", medico);
            return View(medico);
        }

        // GET: Medico/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Log.Information("Acessando a página de edição do médico com ID: {Id}", id);
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                Log.Warning("Médico com ID: {Id} não encontrado para edição.", id);
                return NotFound();
            }
            return View(medico);
        }

        // POST: Medico/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CRM,IdUsuario,IdEspecialidade")] Medico medico)
        {
            if (id != medico.Id)
            {
                Log.Warning("Tentativa de edição de médico com ID inválido. ID esperado: {ExpectedId}, ID recebido: {ReceivedId}", id, medico.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medico);
                    await _context.SaveChangesAsync();
                    Log.Information("Médico atualizado com sucesso: {Medico}", medico);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicoExists(medico.Id))
                    {
                        Log.Warning("Médico com ID: {Id} não encontrado durante atualização.", medico.Id);
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            Log.Warning("ModelState inválido para médico: {Medico}", medico);
            return View(medico);
        }

        // GET: Medico/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            Log.Information("Acessando página de deleção do médico com ID: {Id}", id);
            if (id == null)
            {
                return NotFound();
            }

            var medico = await _context.Medicos
                .Include(m => m.Usuario)
                .Include(m => m.Especialidade)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null)
            {
                Log.Warning("Médico com ID: {Id} não encontrado para deleção.", id);
                return NotFound();
            }

            return View(medico);
        }

        // POST: Medico/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Log.Information("Tentando deletar médico com ID: {Id}", id);
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null)
            {
                Log.Warning("Médico com ID: {Id} não encontrado para deleção.", id);
                return NotFound();
            }
            _context.Medicos.Remove(medico);
            await _context.SaveChangesAsync();
            Log.Information("Médico deletado com sucesso: {Id}", id);
            return RedirectToAction(nameof(Index));
        }

        private bool MedicoExists(int id)
        {
            return _context.Medicos.Any(e => e.Id == id);
        }
    }
}
