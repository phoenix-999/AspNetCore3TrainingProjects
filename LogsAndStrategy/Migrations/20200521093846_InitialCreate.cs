using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LogsAndStrategy.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Blogs",
                columns: table => new
                {
                    BlogId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BlogName = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true, defaultValue: "Author"),
                    Title = table.Column<string>(nullable: true, defaultValue: "Title"),
                    Year = table.Column<int>(nullable: false),
                    Era = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    _id = table.Column<int>(nullable: false, computedColumnSql: "BlogId")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Blogs", x => x.BlogId);
                });

            migrationBuilder.CreateTable(
                name: "Coords",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Radius = table.Column<int>(nullable: false),
                    Lat = table.Column<string>(nullable: true),
                    Lng = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ProductionCoords_Name = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address_Street = table.Column<string>(nullable: true),
                    Address_Number = table.Column<int>(nullable: true),
                    Belay_BelayNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeName = table.Column<string>(nullable: true),
                    EmployeeLastName = table.Column<string>(nullable: true),
                    EmployeeFullName = table.Column<string>(nullable: true, computedColumnSql: "EmployeeName + ', ' + EmployeeLastName"),
                    PassportId = table.Column<string>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    LastPayRaise = table.Column<DateTime>(nullable: true),
                    Description = table.Column<string>(nullable: true, defaultValue: "No description"),
                    RowVersion = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.UniqueConstraint("AK_Employees_Guid", x => x.Guid);
                    table.UniqueConstraint("AK_Employees_PassportId", x => x.PassportId);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    _id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x._id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Shedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Periods = table.Column<string>(nullable: true),
                    ByteProperty = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    PostId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostName = table.Column<string>(nullable: true),
                    BlogId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.PostId);
                    table.ForeignKey(
                        name: "FK_Posts_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "BlogId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    _id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Label = table.Column<string>(nullable: true),
                    Count = table.Column<int>(nullable: false),
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

            migrationBuilder.InsertData(
                table: "Blogs",
                columns: new[] { "BlogId", "BlogName", "Era", "Year" },
                values: new object[] { 11, "Test blog", null, 0 });

            migrationBuilder.InsertData(
                table: "Blogs",
                columns: new[] { "BlogId", "BlogName", "Era", "Year" },
                values: new object[] { 20, "Test blog 2", null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogId",
                table: "Blogs",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_BlogId_BlogName",
                table: "Blogs",
                columns: new[] { "BlogId", "BlogName" },
                unique: true,
                filter: "BlogName is not null");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_BlogId",
                table: "Posts",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Item_id",
                table: "Tags",
                column: "Item_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coords");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Shedules");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Blogs");

            migrationBuilder.DropTable(
                name: "Items");
        }
    }
}
