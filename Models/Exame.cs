using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Exame
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public DateTime Validade { get; set; }
        public string Descricao { get; set; }

        //Relações
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        public Medico Medico { get; set; }

        [ForeignKey(nameof(IdHemograma))]
        public int IdHemograma { get; set; }
        public Hemograma Hemograma { get; set; }



    }
}
