using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ehr_csharp.Models
{
    public class Usuario : IdentityUser
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string Password { get; set; }

    }

    public class EmailSender : IEmailSender<Usuario>
    {
        public Task SendConfirmationLinkAsync(Usuario user, string email, string confirmationLink)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailAsync(Usuario user, string email, string subject, string htmlMessage)
        {
            // Lógica para enviar o e-mail
            return Task.CompletedTask;
        }

        public Task SendPasswordResetCodeAsync(Usuario user, string email, string resetCode)
        {
            throw new NotImplementedException();
        }

        public Task SendPasswordResetLinkAsync(Usuario user, string email, string resetLink)
        {
            throw new NotImplementedException();
        }
    }
}
