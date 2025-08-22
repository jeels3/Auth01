using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_address_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Addressid",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Addressid",
                table: "AspNetUsers",
                column: "Addressid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Address_Addressid",
                table: "AspNetUsers",
                column: "Addressid",
                principalTable: "Address",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Address_Addressid",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Addressid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Addressid",
                table: "AspNetUsers");
        }
    }
}
