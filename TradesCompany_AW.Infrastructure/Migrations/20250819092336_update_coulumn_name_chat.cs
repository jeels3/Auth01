using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradesCompany_AW.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class update_coulumn_name_chat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChannelNames",
                table: "channeldb",
                newName: "ChannelName");

            migrationBuilder.RenameIndex(
                name: "IX_channeldb_ChannelNames",
                table: "channeldb",
                newName: "IX_channeldb_ChannelName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChannelName",
                table: "channeldb",
                newName: "ChannelNames");

            migrationBuilder.RenameIndex(
                name: "IX_channeldb_ChannelName",
                table: "channeldb",
                newName: "IX_channeldb_ChannelNames");
        }
    }
}
