using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Idam.EntityFrameworkCore.Timestamps.Sample.Migrations
{
    /// <inheritdoc />
    public partial class CreateDtCustomTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DtCustoms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 191, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 191, nullable: true),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RemovedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DtCustoms", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DtCustoms");
        }
    }
}
