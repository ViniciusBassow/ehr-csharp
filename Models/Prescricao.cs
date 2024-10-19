using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Prescricao
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }


        //Relações
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        public Medico Medico { get; set; }

        public ICollection<Medicamento> Medicamentos { get; set; }
        //Exemplo: public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents


    }
}
