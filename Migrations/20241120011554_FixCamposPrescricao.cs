using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class FixCamposPrescricao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicamento_Prescricao_PrescricaoId",
                table: "Medicamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Medico_MedicoId",
                table: "Prescricao");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao");

            migrationBuilder.DropIndex(
                name: "IX_Medicamento_PrescricaoId",
                table: "Medicamento");

            migrationBuilder.DropColumn(
                name: "IdMedico",
                table: "Prescricao");

            migrationBuilder.DropColumn(
                name: "PrescricaoId",
                table: "Medicamento");

            migrationBuilder.DropColumn(
                name: "Prescricao",
                table: "Consulta");

            migrationBuilder.RenameColumn(
                name: "IdPaciente",
                table: "Prescricao",
                newName: "IdConsulta");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "Prescricao",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "Prescricao",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdPrescricao",
                table: "Consulta",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prescricao_IdConsulta",
                table: "Prescricao",
                column: "IdConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamento_IdPrescricao",
                table: "Medicamento",
                column: "IdPrescricao");

            migrationBuilder.CreateIndex(
                name: "IX_Consulta_IdPrescricao",
                table: "Consulta",
                column: "IdPrescricao");

            migrationBuilder.AddForeignKey(
                name: "FK_Consulta_Prescricao_IdPrescricao",
                table: "Consulta",
                column: "IdPrescricao",
                principalTable: "Prescricao",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicamento_Prescricao_IdPrescricao",
                table: "Medicamento",
                column: "IdPrescricao",
                principalTable: "Prescricao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Consulta_IdConsulta",
                table: "Prescricao",
                column: "IdConsulta",
                principalTable: "Consulta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Medico_MedicoId",
                table: "Prescricao",
                column: "MedicoId",
                principalTable: "Medico",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao",
                column: "PacienteId",
                principalTable: "Paciente",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consulta_Prescricao_IdPrescricao",
                table: "Consulta");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicamento_Prescricao_IdPrescricao",
                table: "Medicamento");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Consulta_IdConsulta",
                table: "Prescricao");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Medico_MedicoId",
                table: "Prescricao");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao");

            migrationBuilder.DropIndex(
                name: "IX_Prescricao_IdConsulta",
                table: "Prescricao");

            migrationBuilder.DropIndex(
                name: "IX_Medicamento_IdPrescricao",
                table: "Medicamento");

            migrationBuilder.DropIndex(
                name: "IX_Consulta_IdPrescricao",
                table: "Consulta");

            migrationBuilder.DropColumn(
                name: "IdPrescricao",
                table: "Consulta");

            migrationBuilder.RenameColumn(
                name: "IdConsulta",
                table: "Prescricao",
                newName: "IdPaciente");

            migrationBuilder.AlterColumn<int>(
                name: "PacienteId",
                table: "Prescricao",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicoId",
                table: "Prescricao",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdMedico",
                table: "Prescricao",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrescricaoId",
                table: "Medicamento",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Prescricao",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Medicamento_PrescricaoId",
                table: "Medicamento",
                column: "PrescricaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicamento_Prescricao_PrescricaoId",
                table: "Medicamento",
                column: "PrescricaoId",
                principalTable: "Prescricao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Medico_MedicoId",
                table: "Prescricao",
                column: "MedicoId",
                principalTable: "Medico",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescricao_Paciente_PacienteId",
                table: "Prescricao",
                column: "PacienteId",
                principalTable: "Paciente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
