using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using ehr_csharp.Models;
using Microsoft.Extensions.Caching.Memory;

using System.Text;
using System.Drawing;

public class LayoutViewComponent : ViewComponent
{
    
    private readonly IMemoryCache _cache;
    private readonly UserManager<Usuario> _userManager;

    public LayoutViewComponent(IMemoryCache cache, UserManager<Usuario> userManager)
    {
        _cache = cache;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
        {
            byte[] imageBytes = Convert.FromBase64String(UsuarioLogado.ImageByteStr);

            // Adicionando a string Base64 para ser usada diretamente na view
            UsuarioLogado.UserImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";

            return View(UsuarioLogado);
        }

        return View("");
    }
}
