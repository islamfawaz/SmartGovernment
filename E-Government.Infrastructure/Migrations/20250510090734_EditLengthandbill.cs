using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditLengthandbill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExtraFields",
                table: "CivilDocumentRequests",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "{}",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ExtraFields",
                table: "CivilDocumentRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "{}");
        }
    }
}
