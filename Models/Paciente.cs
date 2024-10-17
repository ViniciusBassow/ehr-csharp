using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Paciente
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string Celular { get; set; }
        public string Email{ get; set; }
        public string Cep { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Profissao { get; set; }
        public string EstadoCivil { get; set; }
        public string TelefoneFixo { get; set; }
        public string HistoricoFamiliar { get; set; }
        public string HistoricoFamiliar2 { get; set; }

    }
}
