using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StorageService.Infrastructure.Migrations
{
    public partial class update_userStorage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "UserStorages");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "UserStorages",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "UserStorages");

            migrationBuilder.AddColumn<int>(
                name: "TotalPrice",
                table: "UserStorages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
