using Microsoft.EntityFrameworkCore.Migrations;

namespace LogsAndStrategy.Migrations
{
    public partial class ChangeDeliveryIdName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Deliveries",
                newName: "DeliveryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DeliveryId",
                table: "Deliveries",
                newName: "Id");
        }
    }
}
