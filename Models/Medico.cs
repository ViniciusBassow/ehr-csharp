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
        public string IdUsuario { get; set; }
        [ForeignKey(nameof(IdUsuario))]
        public Usuario Usuario { get; set; }

  

        public int IdEspecialidade { get; set; }
        [ForeignKey(nameof(IdEspecialidade))]
        public Especialidade Especialidade { get; set; }


        public ICollection<Consulta> Consultas { get; set; }
        public ICollection<Diagnostico> Diagnosticos { get; set; }
        public ICollection<Prescricao> Prescricoes { get; set; }



    }
}
