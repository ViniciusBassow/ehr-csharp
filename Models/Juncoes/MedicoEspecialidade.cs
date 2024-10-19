using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ehr_csharp.Models
{
    public class MedicoEspecialidade
    {
        public int IdMedico { get; set; }
        public Medico Medico { get; set; }

        public int IdEspecialidade { get; set; }
        public Especialidade Especialidade { get; set; }
    }

}
