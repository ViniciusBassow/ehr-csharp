using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Medicamento
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        
        public string Medicacao { get; set; }

        public string Dosagem { get; set; }
        public string Intervalo { get; set; }
        public string Duracao { get; set; }

        //Relações
        public int IdPrescricao { get; set; }
        public Prescricao Prescricao { get; set; }



    }
}
