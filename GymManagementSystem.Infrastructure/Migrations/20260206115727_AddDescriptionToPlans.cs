using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagementSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionToPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "MembershipPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "MembershipPlans");
        }
    }
}
