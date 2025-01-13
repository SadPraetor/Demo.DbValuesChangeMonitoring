using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.DbValuesChangeMonitoring.Data.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "configuration",
                table: "ConfigurationValues",
                newName: "Key");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Key",
                schema: "configuration",
                table: "ConfigurationValues",
                newName: "Name");
        }
    }
}
