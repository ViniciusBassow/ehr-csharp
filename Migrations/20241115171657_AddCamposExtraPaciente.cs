using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposExtraPaciente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Altura",
                table: "Paciente",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Paciente",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Carteirinha",
                table: "Paciente",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DataCadastro",
                table: "Paciente",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<float>(
                name: "Peso",
                table: "Paciente",
                type: "real",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Altura",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Carteirinha",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Paciente");

            migrationBuilder.DropColumn(
                name: "Peso",
                table: "Paciente");
        }
    }
}
