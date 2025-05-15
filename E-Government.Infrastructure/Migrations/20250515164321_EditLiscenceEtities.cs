using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditLiscenceEtities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TechnicalInspectionReport",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceDocument",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantNID",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "VehicleLicenseRenewals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "VehicleLicenseRenewals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "VehicleLicenseRenewals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PoliceReport",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DamagedLicensePhoto",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantNID",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "LicenseReplacementRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "LicenseReplacementRequests",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "LicenseReplacementRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RenewalDate",
                table: "DrivingLicenseRenewals",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NewPhoto",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantNID",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApplicantName",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "DrivingLicenseRenewals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateOnly>(
                name: "NewExpiryDate",
                table: "DrivingLicenseRenewals",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PublicId",
                table: "DrivingLicenseRenewals",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "DrivingLicenseRenewals",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantNID",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "VehicleLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "ApplicantNID",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LicenseReplacementRequests");

            migrationBuilder.DropColumn(
                name: "ApplicantNID",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "ApplicantName",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "NewExpiryDate",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "DrivingLicenseRenewals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DrivingLicenseRenewals");

            migrationBuilder.AlterColumn<string>(
                name: "TechnicalInspectionReport",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InsuranceDocument",
                table: "VehicleLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PoliceReport",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DamagedLicensePhoto",
                table: "LicenseReplacementRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RenewalDate",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "NewPhoto",
                table: "DrivingLicenseRenewals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
