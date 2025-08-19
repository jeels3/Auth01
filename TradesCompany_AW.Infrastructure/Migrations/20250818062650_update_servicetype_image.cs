using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_servicetype_image : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgLink",
                table: "serviceTypes",
                newName: "imgLink");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imgLink",
                table: "serviceTypes",
                newName: "ImgLink");
        }
    }
}
