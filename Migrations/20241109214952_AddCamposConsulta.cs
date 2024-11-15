using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExameFisico",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExamesSolicitados",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HipoteseDiagnostica",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HistoricoDoencaAtual",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Orientacoes",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Prescricao",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QueixaPrincipal",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RetornoConsulta",
                table: "Consulta",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExameFisico",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "ExamesSolicitados",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "HipoteseDiagnostica",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "HistoricoDoencaAtual",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "Orientacoes",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "Prescricao",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "QueixaPrincipal",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "RetornoConsulta",
                table: "Consulta");
        }
    }
}
