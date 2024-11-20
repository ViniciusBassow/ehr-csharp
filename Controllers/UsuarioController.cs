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
using System.Text.RegularExpressions;
using Newtonsoft.Json;


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
        public async Task<ActionResult> Index(string sortOrder, string searchString)
        {
            List<Usuario> usuarios = Contexto<Usuario>().Where(x => x.UserName != "root").ToList();

            if (!String.IsNullOrEmpty(searchString))
            {
                usuarios = usuarios.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString)).ToList();
            }

            switch (sortOrder)
            {
                case "name_desc":
                    usuarios = usuarios.OrderByDescending(u => u.UserName).ToList();
                    break;
                case "email":
                    usuarios = usuarios.OrderBy(u => u.Email).ToList();
                    break;
                case "email_desc":
                    usuarios = usuarios.OrderByDescending(u => u.Email).ToList();
                    break;
                default:
                    usuarios = usuarios.OrderBy(u => u.UserName).ToList();
                    break;
            }

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


        public async Task<ActionResult> Editar(Usuario usuario)
        {
            ModelState.Clear();
            RestoreModelStateFromTempData();

            var usuarioBD = Contexto<Usuario>().FirstOrDefault(x => x.Id == usuario.Id);
            if (usuarioBD == null)
                usuarioBD = new Usuario();
            else
            {
                var roles = await _userManager.GetRolesAsync(usuarioBD);
                usuarioBD.Role = roles.FirstOrDefault();
            }

            ViewBag.Especialidades = Contexto<Especialidade>().ToList();
            
            return View(usuarioBD);
        }

        [HttpPost]
        public async Task<ActionResult> Salvar(Usuario usuario)
        {
            ModelState.Clear();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            var usuarioBD = await Contexto<Usuario>().FirstOrDefaultAsync(x => x.Id == usuario.Id);

            ValidarCamposUsuario(usuario, usuarioBD == null);
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Editar", "usuario", null);
            }

            if (usuario.File != null)
                usuario.ImageByteStr = Usuario.Helper.ConverterImagemEmString(usuario.File);

            if (usuarioBD != null)
            {
                usuarioBD.UserName = usuario.UserName;
                usuarioBD.Email = usuario.Email;
                usuarioBD.UpdateAt = DateTime.Now;
                usuarioBD.Name = usuario.Name;
                usuarioBD.Role = usuario.Role;
                usuarioBD.ImageByteStr = usuario.ImageByteStr;

            }
            else
            {
                usuario.PasswordHash = Usuario.Helper.HashPassword(usuario.Password);
                usuario.CreatedAt = DateTime.Now;

                if (usuario.Role == "Medico")
                {
                    usuario.Medico.IdUsuario = usuario.Id;
                }

                errors = await Registrar(usuario);
            }
            SaveChanges();

            if (errors.Any())
            {
                ViewBag.Errors = errors;
                return View("Erro");
            }

            DisplayMensagemSucesso();

            return RedirectToAction("Index", "usuario");
            //return View("Views\\Usuario\\editar.cshtml", usuario);
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
            ModelState.Clear();
            if (string.IsNullOrEmpty(usuario.UserName))
                ModelState.AddModelError(string.Empty, "O campo login é obrigatório.");
            if (string.IsNullOrEmpty(usuario.Password))
                ModelState.AddModelError(string.Empty, "O campo senha é obrigatório.");

            if (!ModelState.IsValid)
                return View("Views\\Login\\index.cshtml");


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

                return RedirectToAction("Index", "Consulta");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Conta bloqueada após várias tentativas.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login ou senha incorretos.");
            }


            return View("Views\\Login\\index.cshtml");
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
                {
                    ModelState.AddModelError("Senha", "O campo Senha é obrigatório");

                }
                else if (Regex.IsMatch(usuario.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"))
                    ModelState.AddModelError("Senha", "O campo Senha deve conter ao menos 8 digitos, sendo eles: 1 caractere maisculo, 1 caractere minúsculo, 1 letra, 1 caractere especial");
            }
            if (usuario.Role == "Medico")
            {
                if (usuario.Medico == null || string.IsNullOrEmpty(usuario.Medico.CRM))
                    ModelState.AddModelError("Cargo", "Os campos CRM é obrigatório");

                if (usuario.Medico.IdEspecialidade == 0)
                    ModelState.AddModelError("Especialidade", "O campo Especialidade é obrigatório");
            }


            var modelStateDict = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToList()
            );

            TempData["ModelState"] = JsonConvert.SerializeObject(modelStateDict);
        }

    }


}
