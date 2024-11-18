using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using Microsoft.Extensions.Caching.Memory;
using SQLApp.Data;
using System.ComponentModel.DataAnnotations;

namespace ehr_csharp.Models
{
    public class Log
    {

        #region Propriedades
        [Key, Column(Order = 0)]
        public int IdLog { get; set; }
        public string TabelaReferencia { get; set; }
        public DateTime DataAlteracao { get; set; }        
        public string IdUsuarioAlteracao{ get; set; }
        public string Alteracao { get; set; }

        #endregion
       
    }




}