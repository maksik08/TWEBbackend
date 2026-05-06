using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBackend.api.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditFieldsAndBusinessConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Suppliers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Suppliers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Customers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("""
                UPDATE [Users]
                SET [UpdatedAt] = SYSUTCDATETIME()
                WHERE [UpdatedAt] = '0001-01-01T00:00:00.0000000';
                """);

            migrationBuilder.Sql("""
                UPDATE [Suppliers]
                SET [CreatedAt] = SYSUTCDATETIME(),
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [CreatedAt] = '0001-01-01T00:00:00.0000000'
                   OR [UpdatedAt] = '0001-01-01T00:00:00.0000000';
                """);

            migrationBuilder.Sql("""
                UPDATE [Products]
                SET [CreatedAt] = SYSUTCDATETIME(),
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [CreatedAt] = '0001-01-01T00:00:00.0000000'
                   OR [UpdatedAt] = '0001-01-01T00:00:00.0000000';
                """);

            migrationBuilder.Sql("""
                UPDATE [Customers]
                SET [CreatedAt] = SYSUTCDATETIME(),
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [CreatedAt] = '0001-01-01T00:00:00.0000000'
                   OR [UpdatedAt] = '0001-01-01T00:00:00.0000000';
                """);

            migrationBuilder.Sql("""
                UPDATE [Categories]
                SET [CreatedAt] = SYSUTCDATETIME(),
                    [UpdatedAt] = SYSUTCDATETIME()
                WHERE [CreatedAt] = '0001-01-01T00:00:00.0000000'
                   OR [UpdatedAt] = '0001-01-01T00:00:00.0000000';
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ContactEmail",
                table: "Suppliers",
                column: "ContactEmail",
                unique: true,
                filter: "[ContactEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suppliers_ContactEmail",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_Name",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Categories");
        }
    }
}
