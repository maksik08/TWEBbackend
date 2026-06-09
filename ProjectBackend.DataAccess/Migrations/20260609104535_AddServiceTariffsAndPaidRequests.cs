using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBackend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceTariffsAndPaidRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "RatedAt",
                table: "ServiceRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rating",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RatingComment",
                table: "ServiceRequests",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceTariffId",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ServiceTariffs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTariffs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_ServiceTariffId",
                table: "ServiceRequests",
                column: "ServiceTariffId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_ServiceTariffs_ServiceTariffId",
                table: "ServiceRequests",
                column: "ServiceTariffId",
                principalTable: "ServiceTariffs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_ServiceTariffs_ServiceTariffId",
                table: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "ServiceTariffs");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_ServiceTariffId",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RatedAt",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "RatingComment",
                table: "ServiceRequests");

            migrationBuilder.DropColumn(
                name: "ServiceTariffId",
                table: "ServiceRequests");
        }
    }
}
