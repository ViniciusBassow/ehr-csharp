using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ehr_csharp.Models
{
    public class Medico
    {
        [Key, Column(Order = 0)]
        public int IdMedico { get; set; }
        public int IdEspecialidade { get; set; }
        public string CrmMedico { get; set; }
        public ICollection<Especialidade> Especialidades { get; set; }

    }
}
