using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shizima.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class _13_UpdateMusicFilePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Musics",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Musics");
        }
    }
}
