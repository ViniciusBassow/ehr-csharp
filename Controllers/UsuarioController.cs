using Azure.Core;
using Azure;
using ehr_csharp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage.Json;


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

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Index()
        {


            List<Usuario> usuarios = Contexto<Usuario>().Where(x => x.UserName != "root").ToList();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuario.Role = roles.FirstOrDefault();
            }

            
            return View(usuarios);
        }

        public async Task<ActionResult> FiltrarUsuarios(string role)
        {
            List<Usuario> usuarios = new List<Usuario> { };
            var userRole = await _roleManager.FindByNameAsync(role);
            
            if (userRole != null)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(userRole.Name);
                var userNames = usersInRole.Select(user => user.UserName).ToList();
                usuarios.AddRange(Contexto<Usuario>().Where(x => userNames.Contains(x.UserName) && x.UserName != "root").ToList());
            }
            else
            {
                usuarios.AddRange(Contexto<Usuario>().Where(x => x.UserName != "root").ToList());
            }

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuario.Role = roles.FirstOrDefault();
            }
            // Caso a role não seja encontrada, retorna uma mensagem de erro
            return PartialView("_ListaUsuario", usuarios);
            
        }


        public async Task<ActionResult> Editar(string Id)
        {

            var usuario = Contexto<Usuario>().FirstOrDefault(x => x.Id == Id);
            if (usuario == null)
                usuario = new Usuario();
            else
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                usuario.Role = roles.FirstOrDefault();
            }

            return View(usuario);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Usuario usuario)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            var usuarioBD = await Contexto<Usuario>().FirstOrDefaultAsync(x => x.Id == usuario.Id);

            if (usuario.File != null)
                usuario.ImageByteStr = Usuario.Helper.ConverterImagemEmString(usuario.File);

            ValidarCamposUsuario(usuario, usuarioBD == null);

 


            if (!ModelState.IsValid)
            {
                return View("Views\\Usuario\\editar.cshtml", new Usuario());
            }

            if (usuarioBD != null)
            {
                usuarioBD.UserName = usuario.UserName;
                usuarioBD.Email = usuario.Email;
                usuarioBD.UpdateAt = DateTime.Now;
                usuarioBD.Name = usuario.Name;
                usuarioBD.Role = usuario.Role;

            }
            else
            {
                usuario.PasswordHash = Usuario.Helper.HashPassword(usuario.Password);
                usuario.CreatedAt = DateTime.Now;
                errors = await Registrar(usuario);
            }
            SaveChanges();

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                return View("Erro");
            }

            return View("Views\\Usuario\\editar.cshtml", usuario);
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


        public void ValidarCamposUsuario(Usuario usuario, bool novo)
        {
            if (string.IsNullOrEmpty(usuario.UserName))
                ModelState.AddModelError("Login", "O campo Login é obrigatório");
            if (string.IsNullOrEmpty(usuario.Email))
                ModelState.AddModelError("Email", "O campo Email é obrigatório");
            if (string.IsNullOrEmpty(usuario.Name))
                ModelState.AddModelError("Nome", "O campo Nome é obrigatório");
            if (string.IsNullOrEmpty(usuario.Role))
                ModelState.AddModelError("Cargo", "O campo Cargo é obrigatório");
            if (novo)
            {
                if (string.IsNullOrEmpty(usuario.Password))
                    ModelState.AddModelError("Senha", "O campo Senha é obrigatório");
            }
        }

    }


}
