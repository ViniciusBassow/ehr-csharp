using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Hemograma
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string Eritrocitos { get; set; }
        public string Hemoglobina { get; set; }
        public string Hematocrito { get; set; }
        public string VCM { get; set; }
        public string HCM{ get; set; }
        public string CHCM { get; set; }
        public string RDW { get; set; }
        //(string? Absoluto, string? Relativo)
        public string Leucocitos { get; set; }
        public string Bastonetes { get; set; }
        public string Segmentados { get; set; }
        public string Eosinofilos { get; set; }
        public string Basofilos { get; set; }
        public string Linfocitos { get; set; }        
        public string Monocitos { get; set; }
        //fim
        public string Plaquetas { get; set; }
        public string VPM { get; set; }
        public string Glicemia { get; set; }
        public string Creatinina { get; set; }
        public string AcidoUrico { get; set; }
        public string Prolactina { get; set; }
        public string Testosterona { get; set; }
        public string ColesterolTotal { get; set; }
        public string HDL { get; set; }
        public string Triglicerides { get; set; }
        public string LDL { get; set; }
        public string NaoHDL { get; set; }

        //Relações
        public int IdExame { get; set; }
        [ForeignKey(nameof(IdExame))]
        public virtual Exame Exame { get; set; }



    }
}
