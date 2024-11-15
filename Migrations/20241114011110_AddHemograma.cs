using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddHemograma : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hemograma_Exame_IdExame",
                table: "Hemograma");

            migrationBuilder.DropIndex(
                name: "IX_Hemograma_IdExame",
                table: "Hemograma");

            migrationBuilder.RenameColumn(
                name: "IdExame",
                table: "Hemograma",
                newName: "IdConsulta");

            migrationBuilder.AddColumn<int>(
                name: "HemogramaId",
                table: "Exame",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Hemograma_IdConsulta",
                table: "Hemograma",
                column: "IdConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_Exame_HemogramaId",
                table: "Exame",
                column: "HemogramaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exame_Hemograma_HemogramaId",
                table: "Exame",
                column: "HemogramaId",
                principalTable: "Hemograma",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Hemograma_Consulta_IdConsulta",
                table: "Hemograma",
                column: "IdConsulta",
                principalTable: "Consulta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exame_Hemograma_HemogramaId",
                table: "Exame");

            migrationBuilder.DropForeignKey(
                name: "FK_Hemograma_Consulta_IdConsulta",
                table: "Hemograma");

            migrationBuilder.DropIndex(
                name: "IX_Hemograma_IdConsulta",
                table: "Hemograma");

            migrationBuilder.DropIndex(
                name: "IX_Exame_HemogramaId",
                table: "Exame");

            migrationBuilder.DropColumn(
                name: "HemogramaId",
                table: "Exame");

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

            migrationBuilder.RenameColumn(
                name: "IdConsulta",
                table: "Hemograma",
                newName: "IdExame");

            migrationBuilder.CreateIndex(
                name: "IX_Hemograma_IdExame",
                table: "Hemograma",
                column: "IdExame",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hemograma_Exame_IdExame",
                table: "Hemograma",
                column: "IdExame",
                principalTable: "Exame",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
