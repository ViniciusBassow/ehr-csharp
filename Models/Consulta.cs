using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SQLApp.Data;
using Microsoft.Extensions.Caching.Memory;

namespace ehr_csharp.Models
{
    public class Consulta
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string? Anamnese { get; set; }
        public string Motivo { get; set; }

        public int StatusConsulta { get; set; }
        public string? MotivoCancelamento { get; set; }

        public string? QueixaPrincipal { get; set; }
        public string? HistoricoDoencaAtual { get; set; }
        public string? ExameFisico { get; set; }
        public string? HipoteseDiagnostica { get; set; }
        public string? ExamesSolicitados { get; set; }
        //public string? Prescricao { get; set; }          
        public string? Orientacoes { get; set; }
        public string? Observacoes { get; set; }
        public DateTime? RetornoConsulta { get; set; }
        public DateTime? DataConclusao { get; set; }



        [NotMapped]
        public TimeSpan Hora { get; set; }

        #region relacionamentos
        //Relações
        public int IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        [ForeignKey(nameof(IdMedico))]
        public Medico Medico { get; set; }
        public int? IdPrescricao { get; set; }
        [ForeignKey(nameof(IdPrescricao))]
        public Prescricao? Prescricao { get; set; }
        #endregion

        #region Configs
        [NotMapped]
        public int? ConfigMinutosAdicionais { get; set; }
        [NotMapped]
        public string? ConfigTemplateCidade { get; set; }
        [NotMapped]
        public string? ConfigTemplateEmail { get; set; }
        [NotMapped]
        public string? ConfigTemplateEstado { get; set; }
        [NotMapped]
        public string? ConfigTemplateLogradouro { get; set; }
        [NotMapped]
        public string? ConfigTemplateNrLogradouro { get; set; }
        [NotMapped]
        public string? ConfigTemplateBairro { get; set; }
        [NotMapped]
        public string? ConfigTemplateTelefone { get; set; }
        [NotMapped]
        public string? ConfigTemplateSiteApoio { get; set; }
        [NotMapped]
        public List<Anexo> Anexos { get; set; }

        public void preencherCamposConfigTemplate(AppDbContext context, IMemoryCache cache)
        {
            GlobalController globalController = new GlobalController(context, cache);
            
            this.ConfigMinutosAdicionais =      (int)globalController.ConsultarConfig("Minutos");
            this.ConfigTemplateCidade =         globalController.ConsultarConfig("TemplateCidade").ToString();
            this.ConfigTemplateEmail =          globalController.ConsultarConfig("TemplateEmail").ToString();
            this.ConfigTemplateEstado =         globalController.ConsultarConfig("TemplateEstado").ToString();
            this.ConfigTemplateLogradouro =     globalController.ConsultarConfig("TemplateLogradouro").ToString();
            this.ConfigTemplateNrLogradouro =   globalController.ConsultarConfig("TemplateNumeroLogradouro").ToString();
            this.ConfigTemplateBairro =         globalController.ConsultarConfig("TemplateBairro").ToString();
            this.ConfigTemplateTelefone =       globalController.ConsultarConfig("TemplateNumeroTelefone").ToString();
            this.ConfigTemplateSiteApoio =      globalController.ConsultarConfig("TemplateUrlSite").ToString();
        }


        #endregion

    }

    public enum StatusConsulta
    {
        Cancelado = 0,
        AguardandoConfirmacao = 1,
        Confirmada = 2,
        EmAndamento = 3,
        Finalizada = 4,
    }
}
