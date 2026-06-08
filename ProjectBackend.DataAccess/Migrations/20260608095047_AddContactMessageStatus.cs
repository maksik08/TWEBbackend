using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectBackend.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddContactMessageStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ContactMessages",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "New");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ContactMessages");
        }
    }
}
