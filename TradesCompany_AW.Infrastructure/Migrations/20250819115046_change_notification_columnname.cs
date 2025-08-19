using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class change_notification_columnname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "notifications",
                newName: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notifications",
                newName: "Created");
        }
    }
}
