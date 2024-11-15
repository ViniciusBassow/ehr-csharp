using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string? Prescricao { get; set; }          
        public string? Orientacoes { get; set; }         
        public string? Observacoes { get; set; }         
        public DateTime? RetornoConsulta { get; set; }  




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
