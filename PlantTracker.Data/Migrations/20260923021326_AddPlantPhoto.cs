using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPlantPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoPath",
                table: "Plants",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoPath",
                table: "Plants");
        }
    }
}
