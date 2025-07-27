using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreationAtProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseRequests_AspNetUsers_ApplicantNID",
                table: "LicenseRequests");

            migrationBuilder.DropIndex(
                name: "IX_LicenseRequests_ApplicantNID",
                table: "LicenseRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantNID",
                table: "LicenseRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "LicenseRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserNID",
                table: "LicenseRequests",
                type: "nvarchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LicenseRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_LicenseRequests_ApplicationUserNID",
                table: "LicenseRequests",
                column: "ApplicationUserNID");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseRequests_AspNetUsers_ApplicationUserNID",
                table: "LicenseRequests",
                column: "ApplicationUserNID",
                principalTable: "AspNetUsers",
                principalColumn: "NID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LicenseRequests_AspNetUsers_ApplicationUserNID",
                table: "LicenseRequests");

            migrationBuilder.DropIndex(
                name: "IX_LicenseRequests_ApplicationUserNID",
                table: "LicenseRequests");

            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "LicenseRequests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserNID",
                table: "LicenseRequests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LicenseRequests");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicantNID",
                table: "LicenseRequests",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseRequests_ApplicantNID",
                table: "LicenseRequests",
                column: "ApplicantNID");

            migrationBuilder.AddForeignKey(
                name: "FK_LicenseRequests_AspNetUsers_ApplicantNID",
                table: "LicenseRequests",
                column: "ApplicantNID",
                principalTable: "AspNetUsers",
                principalColumn: "NID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
