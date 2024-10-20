using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLApp.Data;

public class GlobalController : Controller
{
    protected readonly AppDbContext _context;

    // Injeta o contexto de banco de dados
    public GlobalController(AppDbContext context)
    {
        _context = context;
    }

    // Disponibiliza o DbContext e a opção de usar Set<T>() para qualquer entidade dinamicamente
    protected DbSet<T> Contexto<T>() where T : class
    {
        return _context.Set<T>();
    }
    protected void SaveChanges()
    {
        _context.SaveChanges();
    }
}
