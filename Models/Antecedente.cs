using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Antecedente
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string Descricao { get; set; }
        
        //Relações 

        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

    }
}
