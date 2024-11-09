using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class FixEspecialidadeMedico2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Especialidade_EspecialidadeId",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_EspecialidadeId",
                table: "Medico");

            migrationBuilder.DropColumn(
                name: "EspecialidadeId",
                table: "Medico");

            migrationBuilder.CreateIndex(
                name: "IX_Medico_IdEspecialidade",
                table: "Medico",
                column: "IdEspecialidade");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Especialidade_IdEspecialidade",
                table: "Medico",
                column: "IdEspecialidade",
                principalTable: "Especialidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medico_Especialidade_IdEspecialidade",
                table: "Medico");

            migrationBuilder.DropIndex(
                name: "IX_Medico_IdEspecialidade",
                table: "Medico");

            migrationBuilder.AddColumn<int>(
                name: "EspecialidadeId",
                table: "Medico",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Medico_EspecialidadeId",
                table: "Medico",
                column: "EspecialidadeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medico_Especialidade_EspecialidadeId",
                table: "Medico",
                column: "EspecialidadeId",
                principalTable: "Especialidade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
