using Microsoft.EntityFrameworkCore.Migrations;

namespace LogsAndStrategy.Migrations
{
    public partial class MenyAddresses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address_Number",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Address_Street",
                table: "Deliveries");

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    DeliveryId = table.Column<int>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(nullable: true),
                    Number = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => new { x.DeliveryId, x.Id });
                    table.ForeignKey(
                        name: "FK_Address_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.AddColumn<int>(
                name: "Address_Number",
                table: "Deliveries",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address_Street",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
