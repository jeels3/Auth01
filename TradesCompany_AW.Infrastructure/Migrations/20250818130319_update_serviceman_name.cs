using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_serviceman_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceMen_AspNetUsers_UserId",
                table: "serviceMen");

            migrationBuilder.DropForeignKey(
                name: "FK_serviceMen_serviceTypes_ServiceTypeId",
                table: "serviceMen");

            migrationBuilder.DropPrimaryKey(
                name: "PK_serviceMen",
                table: "serviceMen");

            migrationBuilder.RenameTable(
                name: "serviceMen",
                newName: "serviceMan");

            migrationBuilder.RenameIndex(
                name: "IX_serviceMen_UserId",
                table: "serviceMan",
                newName: "IX_serviceMan_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceMen_ServiceTypeId",
                table: "serviceMan",
                newName: "IX_serviceMan_ServiceTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_serviceMan",
                table: "serviceMan",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceMan_AspNetUsers_UserId",
                table: "serviceMan",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_serviceMan_serviceTypes_ServiceTypeId",
                table: "serviceMan",
                column: "ServiceTypeId",
                principalTable: "serviceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_serviceMan_AspNetUsers_UserId",
                table: "serviceMan");

            migrationBuilder.DropForeignKey(
                name: "FK_serviceMan_serviceTypes_ServiceTypeId",
                table: "serviceMan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_serviceMan",
                table: "serviceMan");

            migrationBuilder.RenameTable(
                name: "serviceMan",
                newName: "serviceMen");

            migrationBuilder.RenameIndex(
                name: "IX_serviceMan_UserId",
                table: "serviceMen",
                newName: "IX_serviceMen_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_serviceMan_ServiceTypeId",
                table: "serviceMen",
                newName: "IX_serviceMen_ServiceTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_serviceMen",
                table: "serviceMen",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_serviceMen_AspNetUsers_UserId",
                table: "serviceMen",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_serviceMen_serviceTypes_ServiceTypeId",
                table: "serviceMen",
                column: "ServiceTypeId",
                principalTable: "serviceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
