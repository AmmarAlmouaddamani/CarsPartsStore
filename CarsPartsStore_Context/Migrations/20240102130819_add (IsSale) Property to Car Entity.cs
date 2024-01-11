using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarsPartsStore_Context.Migrations
{
    /// <inheritdoc />
    public partial class addIsSalePropertytoCarEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSale",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSale",
                table: "Cars");
        }
    }
}
