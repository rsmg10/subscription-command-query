using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SubscriptionQuery.Migrations
{
    /// <inheritdoc />
    public partial class permissionColumnForInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Permission",
                table: "Invitations",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permission",
                table: "Invitations");
        }
    }
}
