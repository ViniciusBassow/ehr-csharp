using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ehr_csharp.Models
{
    public class Especialidade
    {
        [Key, Column(Order = 0)]
        public int IdEspecialidade { get; set; }
        public string NmEspecialidade { get; set; }        

    }
}
