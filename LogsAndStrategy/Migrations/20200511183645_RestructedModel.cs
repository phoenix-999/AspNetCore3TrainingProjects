using Microsoft.EntityFrameworkCore.Migrations;

namespace LogsAndStrategy.Migrations
{
    public partial class RestructedModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "Items");

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    _id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(nullable: true),
                    Item_id = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x._id);
                    table.ForeignKey(
                        name: "FK_Tags_Items_Item_id",
                        column: x => x.Item_id,
                        principalTable: "Items",
                        principalColumn: "_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Item_id",
                table: "Tags",
                column: "Item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "Items",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
