using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Diagnostico
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }

        public string Titulo    { get; set; }
        public string Descricao { get; set; }
        public string Patologia { get; set; }
        public string Especialidade {  get; set; }

        public DateTime Data { get; set; }
        public string CID10 { get; set; }


        //Relações
        public int IdPaciente { get; set; }
        public Paciente Paciente { get; set; }

        public int IdMedico { get; set; }
        public Medico Medico { get; set; }

    }
}
