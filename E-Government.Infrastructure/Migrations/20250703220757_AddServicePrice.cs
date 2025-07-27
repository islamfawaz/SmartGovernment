using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrivingLicenseRenewal");

            migrationBuilder.DropTable(
                name: "LicenseReplacementRequest");

            migrationBuilder.DropTable(
                name: "VehicleLicenseRenewal");

            migrationBuilder.CreateTable(
                name: "ServicePrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ServiceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePrices", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicePrices");

            migrationBuilder.CreateTable(
                name: "DrivingLicenseRenewal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantNID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BillId = table.Column<int>(type: "int", nullable: true),
                    CurrentExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CurrentLicenseNumber = table.Column<int>(type: "int", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MedicalCheckRequired = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewExpiryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NewPhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrivingLicenseRenewal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrivingLicenseRenewal_AspNetUsers_ApplicantNID",
                        column: x => x.ApplicantNID,
                        principalTable: "AspNetUsers",
                        principalColumn: "NID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DrivingLicenseRenewal_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LicenseReplacementRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantNID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BillId = table.Column<int>(type: "int", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DamagedLicensePhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LicenseType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginalLicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PoliceReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ReplacementFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseReplacementRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LicenseReplacementRequest_AspNetUsers_ApplicantNID",
                        column: x => x.ApplicantNID,
                        principalTable: "AspNetUsers",
                        principalColumn: "NID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LicenseReplacementRequest_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VehicleLicenseRenewal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicantNID = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    BillId = table.Column<int>(type: "int", nullable: true),
                    InsuranceDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PendingFines = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PublicId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServiceCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TechnicalInspectionReport = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleRegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleLicenseRenewal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleLicenseRenewal_AspNetUsers_ApplicantNID",
                        column: x => x.ApplicantNID,
                        principalTable: "AspNetUsers",
                        principalColumn: "NID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VehicleLicenseRenewal_Bills_BillId",
                        column: x => x.BillId,
                        principalTable: "Bills",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenseRenewal_ApplicantNID",
                table: "DrivingLicenseRenewal",
                column: "ApplicantNID");

            migrationBuilder.CreateIndex(
                name: "IX_DrivingLicenseRenewal_BillId",
                table: "DrivingLicenseRenewal",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseReplacementRequest_ApplicantNID",
                table: "LicenseReplacementRequest",
                column: "ApplicantNID");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseReplacementRequest_BillId",
                table: "LicenseReplacementRequest",
                column: "BillId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLicenseRenewal_ApplicantNID",
                table: "VehicleLicenseRenewal",
                column: "ApplicantNID");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleLicenseRenewal_BillId",
                table: "VehicleLicenseRenewal",
                column: "BillId");
        }
    }
}
