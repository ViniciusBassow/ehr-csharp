using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Prescricao
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }


        public int IdConsulta { get; set; }
        [ForeignKey(nameof(IdConsulta))]
        public Consulta Consulta { get; set; }

        //public int IdMedico { get; set; }
        //[ForeignKey(nameof(IdMedico))]
        //public Medico Medico { get; set; }


        public IList<Medicamento> Medicamentos { get; set; }

        //Exemplo: public ICollection<Post> Posts { get; } = new List<Post>(); // Collection navigation containing dependents

        [NotMapped]
        public IList<Anexo> Anexos { get; set; }
    }
}
