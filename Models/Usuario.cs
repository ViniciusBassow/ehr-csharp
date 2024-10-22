using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;

namespace ehr_csharp.Models
{
    public class Usuario : IdentityUser
    {
        #region Propriedades
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string ImageByteStr { get; set; }
        public Medico? Medico { get; set; } 
        #endregion

        #region ViewModel Propriedades
        [NotMapped]
        public string? Role { get; set; } 
        [NotMapped]
        public string? Password { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        [NotMapped]
        public bool RememberMe { get; set; }
        [NotMapped]
        public string? UserImageBase64 { get; set; }
        #endregion

        #region Métodos Utilitários
        public static class Helper
        {
            public static string HashPassword(string password)
            {
                byte[] salt;
                byte[] buffer2;

                if (password == null)
                {
                    throw new ArgumentNullException("password");
                }

                using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
                {
                    salt = bytes.Salt;
                    buffer2 = bytes.GetBytes(0x20);
                }

                byte[] dst = new byte[0x31];
                Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
                Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
                return Convert.ToBase64String(dst);
            }

            public static string ConverterImagemEmString(IFormFile file)
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyToAsync(memoryStream);
                    byte[] fileBytes = memoryStream.ToArray();
                    return Convert.ToBase64String(fileBytes);
                }
            }
        }
        #endregion
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
