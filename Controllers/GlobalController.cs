using ehr_csharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SQLApp.Data;
using System.Net.Mail;
using System.Net;

public class GlobalController : Controller
{


    protected readonly AppDbContext _context;
    private readonly IMemoryCache _cache;

    // Injeta o contexto de banco de dados
    public GlobalController(AppDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewBag.UserRole = _cache.Get<string>("UserRole"); ;
    }

    // Disponibiliza o DbContext e a opção de usar Set<T>() para qualquer entidade dinamicamente
    protected DbSet<T> Contexto<T>() where T : class
    {
        return _context.Set<T>();
    }
    protected void SaveChanges()
    {
        _context.SaveChanges();
    }

    public void DisplayMensagemSucesso(string message = "Registro salvo com sucesso!")
    {
        TempData["MensagemSucesso"] = message;
    }

    public void RestoreModelStateFromTempData()
    {
        if (TempData.ContainsKey("ModelState"))
        {
            var modelStateJson = TempData["ModelState"] as string;
            if (modelStateJson != null)
            {
                var modelStateDict = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(modelStateJson);

                foreach (var key in modelStateDict.Keys)
                {
                    foreach (var error in modelStateDict[key])
                    {
                        ModelState.AddModelError(key, error);
                    }
                }
            }
        }
    }

    public object ConsultarConfig(string idConfig)
    {
        var config = Contexto<Config>().FirstOrDefault(x => x.IdConfig == idConfig);

        if (config != null)
            switch (config.IdTipoParametro)
            {
                case (int)TipoParametro.String:
                    return config.Valor;
                case (int)TipoParametro.Int:
                    return int.Parse(config.Valor);
                case (int)TipoParametro.DateTime:
                    return DateTime.Parse(config.Valor);
                default: return "";
            }
        else
            return "";
    }

    public void RegistrarLog(string textoAlteração, string TabelaReferencia)
    {
        if (_cache.TryGetValue("UsuarioLogado", out Usuario usuarioLogado))
        {
            Log log = new Log()
            {
                DataAlteracao = DateTime.Now,
                TabelaReferencia = TabelaReferencia,
                Alteracao = textoAlteração,
                IdUsuarioAlteracao = usuarioLogado.Id
            };

            Contexto<Log>().Add(log);
            SaveChanges();
        }
    }

    public bool EnviarEmail(string destinatario, string assunto, string corpo)
    {        
        var configs = Contexto<Config>().Where(x => x.IdConfig.Contains("Email")).ToList();

        string remetente = configs.FirstOrDefault(x => x.IdConfig == "EmailRemetente").Valor;
        string senha = configs.FirstOrDefault(x => x.IdConfig == "EmailSenha").Valor;
        string smtpServidor = configs.FirstOrDefault(x => x.IdConfig == "EmailServidor").Valor;
        int portaSmtp = int.Parse(configs.FirstOrDefault(x => x.IdConfig == "EmailPorta").Valor);
        bool habilitarSSL = configs.FirstOrDefault(x => x.IdConfig == "EmailSsl").Valor == "1";
        string anexoCaminho = null;


        using (SmtpClient smtpClient = new SmtpClient(smtpServidor, portaSmtp))
        {
            

            smtpClient.Credentials = new NetworkCredential(remetente, senha);
            smtpClient.EnableSsl = habilitarSSL;

            // Criar o e-mail
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(remetente);
                mailMessage.To.Add(destinatario);
                mailMessage.Subject = assunto;
                mailMessage.Body = corpo;
                mailMessage.IsBodyHtml = true; // Caso queira enviar HTML

                // Anexar arquivo (opcional)
                if (!string.IsNullOrEmpty(anexoCaminho))
                {
                    Attachment attachment = new Attachment(anexoCaminho);
                    mailMessage.Attachments.Add(attachment);
                }

                // Enviar e-mail
                smtpClient.Send(mailMessage);
            }
        }

        return true; // E-mail enviado com sucesso
    
      
    }


}
