using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Config
    {
        [Key, Column(Order = 0)]
        public string IdConfig { get; set; }
        public string Descricao { get; set; }
        public string Valor { get; set; }
        public int IdTipoParametro { get; set; }
    }

    public enum TipoParametro
    {        
        String = 1,
        Int = 2,
        DateTime = 3        
    }
}

