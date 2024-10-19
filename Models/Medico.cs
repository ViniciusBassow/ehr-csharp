using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ehr_csharp.Models
{
    public class Medico
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string CRM { get; set; }

        //Relações
        public int IdUsuario { get; set; }
        public Usuario Usuario { get; set; }

        public int IdEspecialidade { get; set; }
        public Especialidade Especialidade { get; set; }

        public ICollection<Consulta> Consultas { get; set; }
        public ICollection<Diagnostico> Diagnosticos { get; set; }
        public ICollection<Prescricao> Prescricoes { get; set; }



    }
}
