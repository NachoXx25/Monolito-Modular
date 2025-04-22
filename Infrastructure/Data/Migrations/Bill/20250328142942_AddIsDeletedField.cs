using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monolito_Modular.Infrastructure.Data.Migrations.Bill
{
    /// <inheritdoc />
    public partial class AddIsDeletedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Bills",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Bills");
        }
    }
}
