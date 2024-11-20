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

        //[Authorize(Roles = "Admin")]
        [CustomAuthorize("s")]
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

            // Log de início de edição de usuário
            Log logIniciarEdicao = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = usuarioBD == null ? $"Início de criação do usuário com ID {usuario.Id}" : $"Início de edição do usuário com ID {usuario.Id}"
            };
            //Contexto<Log>().Add(logIniciarEdicao); // Adicionando log

            if (usuario.File != null)
                usuario.ImageByteStr = Usuario.Helper.ConverterImagemEmString(usuario.File);

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

                // Log de atualização de usuário
                Log logAtualizarUsuario = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Atualização do usuário com ID {usuario.Id}"
                };
                //Contexto<Log>().Add(logAtualizarUsuario); // Adicionando log

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

                // Log de criação de usuário
                Log logCriarUsuario = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Criação de novo usuário com ID {usuario.Id}"
                };
                //Contexto<Log>().Add(logCriarUsuario); // Adicionando log
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
                // Log de início de edição ou criação de usuário
                Log logIniciar = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = usuario.Id == null ? $"Início de criação do usuário com nome {usuario.UserName}" : $"Início de edição do usuário com ID {usuario.Id}"
                };
                //Contexto<Log>().Add(logIniciar); // Adicionando log

                // Log de adição de arquivo ao usuário (caso tenha arquivo)
                if (usuario.File != null)
                {
                    Log logAdicionarArquivo = new Log()
                    {
                        DataAlteracao = DateTime.Now,
                        TabelaReferencia = "Usuario",
                        Alteracao = $"Adição de arquivo ao usuário com ID {usuario.Id}"
                    };
                    //Contexto<Log>().Add(logAdicionarArquivo); // Adicionando log
                }

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

            // Log de início de login
            Log logLogin = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = $"Início de login para o usuário {usuario.UserName}"
            };
            //Contexto<Log>().Add(logLogin); // Adicionando log

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

                // Log de sucesso no login
                Log logLoginSuccess = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Login bem-sucedido para o usuário {usuario.UserName}"
                };
                //Contexto<Log>().Add(logLoginSuccess); // Adicionando log

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

            // Log de falha no login
            Log logLoginFailure = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = $"Falha no login para o usuário {usuario.UserName}. Motivo: {(result.IsLockedOut ? "Conta bloqueada" : "Login ou senha incorretos")}"
            };
            //Contexto<Log>().Add(logLoginFailure); // Adicionando log

            return View("Views\\Login\\index.cshtml");
        }

        public void ValidarCamposUsuario(Usuario usuario, bool novo)
        {
            // Log de início de validação de campos (equivalente ao início da edição de um usuário)
            Log logValidacao = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = $"Início de validação de campos para o usuário {usuario.UserName}. Novo: {novo}"
            };
            //Contexto<Log>().Add(logValidacao); // Adicionando log

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var usuarioLogado = await _userManager.FindByIdAsync(userId);

            if (usuarioLogado != null)
            {
                _cache.Remove("UsuarioLogado");   
                SaveChanges();
            }


            await _signInManager.SignOutAsync();


            return RedirectToAction("Index", "Home");
        }

        public ActionResult AcessoNegado()
        {

            return View();
        }
    }


}
