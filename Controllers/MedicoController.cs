using Microsoft.AspNetCore.Mvc;
using ehr_csharp.Models;  // Certifique-se de que a classe Agendamento está no namespace correto
using System.Collections.Generic;  // Necessário para a lista de agendamentos em memória

namespace ehr_csharp.Controllers
{
    [Route("medico")]
    public class MedicoController : Controller
    {
        // Lista temporária de agendamentos (simulação sem banco de dados)
        private static List<Agendamento> Agendamentos = new List<Agendamento>();

        // Rota para Dashboard
        [Route("dash")]
        public IActionResult Dash()
        {
            return View("Dash"); // Passando o nome do médico para a view "Dash"
        }

        // Rota para exibir agendamentos e o formulário para novo agendamento
        [Route("agendamento")]
        public IActionResult Agendamento()
        {
            // Exibe a lista de agendamentos na view
            ViewBag.Agendamentos = Agendamentos;
            return View();
        }

        // Processa a criação do agendamento (sem banco de dados, adiciona à lista em memória)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("agendamento")]
        public IActionResult Agendamento(Agendamento agendamento)
        {
            if (ModelState.IsValid)
            {
                // Adiciona o novo agendamento à lista
                Agendamentos.Add(agendamento);

                // Redireciona para a mesma página para mostrar o novo agendamento
                return RedirectToAction(nameof(Agendamento));
            }

            // Se o modelo não for válido, retorna à mesma página com os erros de validação
            return View(agendamento);
        }
    }
}
