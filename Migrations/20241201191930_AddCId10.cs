using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ehr_csharp.Migrations
{
    /// <inheritdoc />
    public partial class AddCId10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Cid",
                table: "Consulta",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cid",
                table: "Consulta");
        }
    }
}
