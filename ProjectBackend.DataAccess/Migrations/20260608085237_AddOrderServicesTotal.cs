using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBackend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderServicesTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ServicesTotal",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServicesTotal",
                table: "Orders");
        }
    }
}
