using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;
    private readonly string _redirectPage;

    public CustomAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;        
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Verifica se o usuário está autenticado
        if (context.HttpContext.User.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
        {
            // Redireciona para a página personalizada de login ou não autorizado
            context.Result = new RedirectToActionResult("Index", "Home", null);
            return;
        }

        // Verifica as roles necessárias
        if (_roles != null && _roles.Any() && !_roles.Any(role => context.HttpContext.User.IsInRole(role)))
        {
            // Redireciona para a página personalizada de acesso negado
            context.Result = new RedirectToActionResult("AcessoNegado", "Usuario", new { area = "" });
        }
    }
}
