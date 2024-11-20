using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class FixRelacaoPresricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao");

            migrationBuilder.DropIndex(
                name: "IX_Prescricao_PacienteId",
                table: "Prescricao");

            migrationBuilder.DropColumn(
                name: "PacienteId",
                table: "Prescricao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PacienteId",
                table: "Prescricao",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescricao_PacienteId",
                table: "Prescricao",
                column: "PacienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao",
                column: "PacienteId",
                principalTable: "Paciente",
                principalColumn: "Id");
        }
    }
}
