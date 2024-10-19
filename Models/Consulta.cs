using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Consulta
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string Data { get; set; }
        public string Anamnese { get; set; }
        public string Motivo { get; set; }
        
        //Relações
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        public Medico Medico { get; set; }

        public int IdDiagnostico { get; set; }
        public Diagnostico Diagnostico { get; set; }

    }
}
