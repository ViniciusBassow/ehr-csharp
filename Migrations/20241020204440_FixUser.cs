using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class FixUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UsuarioId",
                table: "AspNetUserRoles");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUserRoles_UsuarioId",
                table: "AspNetUserRoles");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "AspNetUserRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "AspNetUserRoles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_UsuarioId",
                table: "AspNetUserRoles",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UsuarioId",
                table: "AspNetUserRoles",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
