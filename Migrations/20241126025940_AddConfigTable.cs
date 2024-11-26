using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddConfigTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Config",
                columns: table => new
                {
                    IdConfig = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdTipoParametro = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config", x => x.IdConfig);
                });

            migrationBuilder.InsertData(
                table: "Config",
                columns: new[] { "IdConfig", "Descricao", "Valor", "IdTipoParametro" },
                values: new object[,]
                {
                    { "TemplateEstado", "Template - Estado", "SP", 1 },
                    { "TemplateCidade", "Template - Cidade", "Americana", 1 },
                    { "TemplateLogradouro", "Template - Logradouro", "Rua da Paz", 2 },
                    { "TemplateNumeroLogradouro", "Template - Número do Logradouro", "123", 1 },
                    { "TemplateNumeroTelefone", "Template - Número de Telefone", "(19) 3434-2020", 1 },
                    { "TemplateEmail", "Template - E-mail", "clinica@dominio.com", 1 },
                    { "TemplateUrlSite", "Template - Url Site de apoio", "www.siteclinica.com.br", 1 },
                    { "Minutos", "Documentos - Define a quantidade de minutos adicionais que será incluída ao tempo da consulta médica ", "15", 2 }
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Config");
        }
    }
}
