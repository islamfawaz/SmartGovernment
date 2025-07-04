using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_Government.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullabe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_LicenseRequests_RequestId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_RequestId",
                table: "Bills");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LicenseRequests",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7240));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7245));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7246));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7248));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7249));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7250));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7251));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 4, 16, 49, 31, 355, DateTimeKind.Utc).AddTicks(7252));

            migrationBuilder.CreateIndex(
                name: "IX_Bills_RequestId",
                table: "Bills",
                column: "RequestId",
                unique: true,
                filter: "[RequestId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_LicenseRequests_RequestId",
                table: "Bills",
                column: "RequestId",
                principalTable: "LicenseRequests",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bills_LicenseRequests_RequestId",
                table: "Bills");

            migrationBuilder.DropIndex(
                name: "IX_Bills_RequestId",
                table: "Bills");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "LicenseRequests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Bills",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8374));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8383));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8385));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8386));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8387));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8389));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 7,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8390));

            migrationBuilder.UpdateData(
                table: "ServicePrices",
                keyColumn: "Id",
                keyValue: 8,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 22, 18, 59, 344, DateTimeKind.Utc).AddTicks(8391));

            migrationBuilder.CreateIndex(
                name: "IX_Bills_RequestId",
                table: "Bills",
                column: "RequestId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_LicenseRequests_RequestId",
                table: "Bills",
                column: "RequestId",
                principalTable: "LicenseRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
