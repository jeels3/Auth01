using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_chat_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "channeldb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelNames = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatorId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channeldb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_channeldb_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "channelMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    ChanneldbId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channelMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_channelMessages_AspNetUsers_SenderId",
                        column: x => x.SenderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_channelMessages_channeldb_ChanneldbId",
                        column: x => x.ChanneldbId,
                        principalTable: "channeldb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "channelUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelId = table.Column<int>(type: "int", nullable: false),
                    ChanneldbId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_channelUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_channelUsers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_channelUsers_channeldb_ChanneldbId",
                        column: x => x.ChanneldbId,
                        principalTable: "channeldb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "isSeens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChannelMessageId = table.Column<int>(type: "int", nullable: false),
                    Seen = table.Column<bool>(type: "bit", nullable: false),
                    SeenDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReceiverId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_isSeens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_isSeens_AspNetUsers_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_isSeens_channelMessages_ChannelMessageId",
                        column: x => x.ChannelMessageId,
                        principalTable: "channelMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_channeldb_ChannelNames",
                table: "channeldb",
                column: "ChannelNames",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_channeldb_UserId",
                table: "channeldb",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_channelMessages_ChanneldbId",
                table: "channelMessages",
                column: "ChanneldbId");

            migrationBuilder.CreateIndex(
                name: "IX_channelMessages_SenderId",
                table: "channelMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_channelUsers_ChanneldbId",
                table: "channelUsers",
                column: "ChanneldbId");

            migrationBuilder.CreateIndex(
                name: "IX_channelUsers_UserId",
                table: "channelUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_isSeens_ChannelMessageId",
                table: "isSeens",
                column: "ChannelMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_isSeens_ReceiverId",
                table: "isSeens",
                column: "ReceiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "channelUsers");

            migrationBuilder.DropTable(
                name: "isSeens");

            migrationBuilder.DropTable(
                name: "channelMessages");

            migrationBuilder.DropTable(
                name: "channeldb");
        }
    }
}
