using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Hemograma
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string? Eritrocitos { get; set; }
        public string? Hemoglobina { get; set; }
        public string? Hematocrito { get; set; }
        public string? VCM { get; set; }
        public string? HCM { get; set; }
        public string? CHCM { get; set; }
        public string? RDW { get; set; }

        // Leucócitos e suas subcategorias (Absoluto e Relativo)
        public string? Leucocitos_Absoluto { get; set; }
        public string? Leucocitos_Relativo { get; set; }
        public string? Bastonetes_Absoluto { get; set; }
        public string? Bastonetes_Relativo { get; set; }
        public string? Segmentados_Absoluto { get; set; }
        public string? Segmentados_Relativo { get; set; }
        public string? Eosinofilos_Absoluto { get; set; }
        public string? Eosinofilos_Relativo { get; set; }
        public string? Basofilos_Absoluto { get; set; }
        public string? Basofilos_Relativo { get; set; }
        public string? Linfocitos_Absoluto { get; set; }
        public string? Linfocitos_Relativo { get; set; }
        public string? Monocitos_Absoluto { get; set; }
        public string? Monocitos_Relativo { get; set; }

        // Outros campos
        public string? Plaquetas { get; set; }
        public string? VPM { get; set; }
        public string? Glicemia { get; set; }
        public string? Creatinina { get; set; }
        public string? AcidoUrico { get; set; }
        public string? Prolactina { get; set; }
        public string? Testosterona { get; set; }
        public string? ColesterolTotal { get; set; }
        public string? HDL { get; set; }
        public string? Triglicerides { get; set; }
        public string? LDL { get; set; }
        public string? NaoHDL { get; set; }

        //Relações
        public int IdConsulta { get; set; }

        [ForeignKey(nameof(IdConsulta))]
        public Consulta Consulta { get; set; }



    }
}
