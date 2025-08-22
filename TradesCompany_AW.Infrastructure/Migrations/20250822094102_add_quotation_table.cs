using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class add_quotation_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quotations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    ServiceManId = table.Column<int>(type: "int", nullable: false),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quotations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_quotations_customerBookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "customerBookings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_quotations_serviceMan_ServiceManId",
                        column: x => x.ServiceManId,
                        principalTable: "serviceMan",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quotations_BookingId",
                table: "quotations",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_quotations_ServiceManId",
                table: "quotations",
                column: "ServiceManId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quotations");
        }
    }
}
