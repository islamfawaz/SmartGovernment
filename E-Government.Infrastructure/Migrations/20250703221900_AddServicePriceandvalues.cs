using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddServicePriceandvalues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ServicePrices",
                columns: new[] { "Id", "CreatedAt", "Currency", "IsActive", "Price", "ServiceCode", "ServiceName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8374), "EGP", true, 100.00m, "DRIVING_RENEW", "تجديد رخصة القيادة" },
                    { 2, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8383), "EGP", true, 120.00m, "DRIVING_REPLACE_LOST", "بدل فاقد لرخصة القيادة" },
                    { 3, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8385), "EGP", true, 200.00m, "DRIVING_NEW", "استخراج رخصة قيادة جديدة" },
                    { 4, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8386), "EGP", true, 80.00m, "LICENSE_DIGITAL", "رخصة رقمية" },
                    { 5, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8387), "EGP", true, 150.00m, "VEHICLE_RENEW", "تجديد رخصة مركبة" },
                    { 6, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8389), "EGP", true, 180.00m, "VEHICLE_NEW", "رخصة مركبة جديدة" },
                    { 7, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8390), "EGP", true, 0.00m, "TRAFFIC_FINE_VIEW", "الاستعلام عن مخالفات المرور" },
                    { 8, new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8391), "EGP", true, 50.00m, "TRAFFIC_FINE_PAY", "دفع مخالفات المرور" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
