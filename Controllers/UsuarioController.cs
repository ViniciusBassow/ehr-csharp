using ehr_csharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;


namespace ehr_csharp.Controllers
{
    public class UsuarioController : GlobalController
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;
        private readonly IMemoryCache _cache;

        public UsuarioController(AppDbContext context, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, SignInManager<Usuario> signInManager, IMemoryCache cache) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _cache = cache;
        }

        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> Index()
        {
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);            
            //var userName = User.Identity.Name;            
            //var teste = await _userManager.FindByIdAsync(userId);

            List<Usuario> usuarios = Contexto<Usuario>().ToList();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                var role = roles.FirstOrDefault(); // Considerando que cada usuário tenha apenas uma role (ou pegar a primeira)
                usuario.Role = role;

                if (!string.IsNullOrEmpty(usuario.ImageByteStr))
                {
                    byte[] imageBytes = Convert.FromBase64String(usuario.ImageByteStr);

                    usuario.UserImageBase64 = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                }
            }

            return View(usuarios);
        }

        public ActionResult Editar(string Id)
        {
            var usuario = Contexto<Usuario>().FirstOrDefault(x => x.Id == Id);
            if (usuario == null)
                usuario = new Usuario();

            return View(usuario);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Usuario usuario)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();


            usuario.PasswordHash = Usuario.Helper.HashPassword(usuario.Password);
            usuario.ImageByteStr = Usuario.Helper.ConverterImagemEmString(usuario.File);


            var usuarioBD = await Contexto<Usuario>().FirstOrDefaultAsync(x => x.Id == usuario.Id);

            if (usuarioBD != null)
            {
                usuarioBD.UserName = usuario.UserName;
                usuarioBD.Email = usuario.Email;
                usuarioBD.UpdateAt = DateTime.Now;

            }
            else
            {

                usuario.CreatedAt = DateTime.Now;
                errors = await Registrar(usuario);
            }


            SaveChanges();


            if (errors.Any())
            {
                ViewBag.Errors = errors;
                return View("Erro");
            }

            return View(usuario);
        }

        [HttpPost]
        public async Task<Dictionary<string, string>> Registrar(Usuario usuario)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            try
            {

                var result = await _userManager.CreateAsync(usuario, usuario.Password);

                if (result.Succeeded)
                {

                    var addToRoleResult = await _userManager.AddToRoleAsync(usuario, usuario.Role);

                    if (!addToRoleResult.Succeeded)
                    {
                        foreach (var error in addToRoleResult.Errors)
                        {
                            errors.Add(string.Empty, error.Description);
                        }
                    }
                }
                else
                {

                    foreach (var error in result.Errors)
                    {
                        errors.Add(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {

                errors.Add("Exception", ex.Message);
            }

            return errors;
        }

        [HttpPost]
        public async Task<IActionResult> Login(Usuario usuario)
        {

            var result = await _signInManager.PasswordSignInAsync(usuario.UserName, usuario.Password, usuario.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!_cache.TryGetValue("UsuarioLogado", out Usuario UsuarioLogado))
                {

                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    UsuarioLogado = await _userManager.FindByIdAsync(userId);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // expira se não for acessado em 5 minutos

                    _cache.Set("UsuarioLogado", UsuarioLogado, cacheEntryOptions);
                }

                return RedirectToAction("Index", "Usuario");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Conta bloqueada após várias tentativas.");
            }
            else
            {

                ModelState.AddModelError(string.Empty, "Login ou senha incorretos.");
            }


            return View();
        }



    }


}
