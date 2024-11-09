using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Anexo
    {
        [Key, Column(Order = 0)]
        public int IdAnexo { get; set; }
        public string NomeArquivo { get; set; }
        public byte[] ArquivoData { get; set; }
        public string TipoArquivo { get; set; }

        public bool Ativo { get; set; }        



        public string IdTabelaReferencia { get; set; }
        public string NmTabelaReferencia { get; set; }


    }
}
