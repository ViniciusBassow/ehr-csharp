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
        [NotMapped]
        public TimeSpan Hora { get; set; }

        //Relações
        public int IdPaciente { get; set; }
        [ForeignKey(nameof(IdPaciente))]
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        [ForeignKey(nameof(IdMedico))]
        public Medico Medico { get; set; }

        public int StatusConsulta { get; set; }
        public string? MotivoCancelamento { get; set; }
        //public int IdDiagnostico { get; set; }
        //[ForeignKey(nameof(IdDiagnostico))]
        //public Diagnostico Diagnostico { get; set; }

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
