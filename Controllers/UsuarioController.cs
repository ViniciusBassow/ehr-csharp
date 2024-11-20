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
            // Caso a role n�o seja encontrada, retorna uma mensagem de erro
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

            // Log de in�cio de edi��o de usu�rio
            Log logIniciarEdicao = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = usuarioBD == null ? $"In�cio de cria��o do usu�rio com ID {usuario.Id}" : $"In�cio de edi��o do usu�rio com ID {usuario.Id}"
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

                // Log de atualiza��o de usu�rio
                Log logAtualizarUsuario = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Atualiza��o do usu�rio com ID {usuario.Id}"
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

                // Log de cria��o de usu�rio
                Log logCriarUsuario = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Cria��o de novo usu�rio com ID {usuario.Id}"
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
                // Log de in�cio de edi��o ou cria��o de usu�rio
                Log logIniciar = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = usuario.Id == null ? $"In�cio de cria��o do usu�rio com nome {usuario.UserName}" : $"In�cio de edi��o do usu�rio com ID {usuario.Id}"
                };
                //Contexto<Log>().Add(logIniciar); // Adicionando log

                // Log de adi��o de arquivo ao usu�rio (caso tenha arquivo)
                if (usuario.File != null)
                {
                    Log logAdicionarArquivo = new Log()
                    {
                        DataAlteracao = DateTime.Now,
                        TabelaReferencia = "Usuario",
                        Alteracao = $"Adi��o de arquivo ao usu�rio com ID {usuario.Id}"
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
                ModelState.AddModelError(string.Empty, "O campo login � obrigat�rio.");
            if (string.IsNullOrEmpty(usuario.Password))
                ModelState.AddModelError(string.Empty, "O campo senha � obrigat�rio.");

            if (!ModelState.IsValid)
                return View("Views\\Login\\index.cshtml");

            // Log de in�cio de login
            Log logLogin = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = $"In�cio de login para o usu�rio {usuario.UserName}"
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
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30)); // expira se n�o for acessado em 5 minutos

                    _cache.Set("UsuarioLogado", UsuarioLogado, cacheEntryOptions);
                }

                // Log de sucesso no login
                Log logLoginSuccess = new Log()
                {
                    DataAlteracao = DateTime.Now,
                    TabelaReferencia = "Usuario",
                    Alteracao = $"Login bem-sucedido para o usu�rio {usuario.UserName}"
                };
                //Contexto<Log>().Add(logLoginSuccess); // Adicionando log

                return RedirectToAction("Index", "Consulta");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Conta bloqueada ap�s v�rias tentativas.");
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
                Alteracao = $"Falha no login para o usu�rio {usuario.UserName}. Motivo: {(result.IsLockedOut ? "Conta bloqueada" : "Login ou senha incorretos")}"
            };
            //Contexto<Log>().Add(logLoginFailure); // Adicionando log

            return View("Views\\Login\\index.cshtml");
        }

        public void ValidarCamposUsuario(Usuario usuario, bool novo)
        {
            // Log de in�cio de valida��o de campos (equivalente ao in�cio da edi��o de um usu�rio)
            Log logValidacao = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = "Usuario",
                Alteracao = $"In�cio de valida��o de campos para o usu�rio {usuario.UserName}. Novo: {novo}"
            };
            //Contexto<Log>().Add(logValidacao); // Adicionando log

            if (string.IsNullOrEmpty(usuario.UserName))
                ModelState.AddModelError("Login", "O campo Login � obrigat�rio");
            if (string.IsNullOrEmpty(usuario.Email))
                ModelState.AddModelError("Email", "O campo Email � obrigat�rio");
            if (string.IsNullOrEmpty(usuario.Name))
                ModelState.AddModelError("Nome", "O campo Nome � obrigat�rio");
            if (string.IsNullOrEmpty(usuario.Role))
                ModelState.AddModelError("Cargo", "O campo Cargo � obrigat�rio");
            if (novo)
            {
                if (string.IsNullOrEmpty(usuario.Password))
                {
                    ModelState.AddModelError("Senha", "O campo Senha � obrigat�rio");

                }
                else if (Regex.IsMatch(usuario.Password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$"))
                    ModelState.AddModelError("Senha", "O campo Senha deve conter ao menos 8 digitos, sendo eles: 1 caractere maisculo, 1 caractere min�sculo, 1 letra, 1 caractere especial");
            }
            if (usuario.Role == "Medico")
            {
                if (usuario.Medico == null || string.IsNullOrEmpty(usuario.Medico.CRM))
                    ModelState.AddModelError("Cargo", "Os campos CRM � obrigat�rio");

                if (usuario.Medico.IdEspecialidade == 0)
                    ModelState.AddModelError("Especialidade", "O campo Especialidade � obrigat�rio");
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
