using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;

namespace ehr_csharp.Models
{
    public class Paciente
    {
        [Key, Column(Order = 0)]
        public int Id { get; set; }
        public string NomeCompleto { get; set; }
        public string Sexo { get; set; }
        public string Endereco { get; set; }
        public string? Celular { get; set; }
        public string? Email { get; set; }
        public string Cep { get; set; }
        public string Cpf { get; set; }
        public string Rg { get; set; }
        public DateOnly DataNascimento { get; set; }
        public string Profissao { get; set; }
        public string EstadoCivil { get; set; }
        public string? TelefoneFixo { get; set; }
        public string? HistoricoFamiliar { get; set; } //Mudar para somente Historico e colocar como uma lista?
        [NotMapped]
        public bool Editar
        {
            get
            {
                // Verifica se há uma string de imagem válida
                if (Consultas == null || (Consultas.Any(x => DateTime.Now >= x.Data && x.StatusConsulta == (int)StatusConsulta.EmAndamento)))
                    return true;
                else
                    return false;
            }
        }

        //Relações
        public virtual IList<Antecedente>? Antecedentes { get; set; }
        public ICollection<Consulta>? Consultas { get; set; }
        public ICollection<Diagnostico>? Diagnosticos { get; set; }
        public ICollection<Prescricao>? Prescricoes { get; set; }

        [NotMapped]
        public Hemograma ultimaConsultaHemograma { get; set; }

        [NotMapped]
        public List<Anexo> Anexos { get; set; }

    }
}
