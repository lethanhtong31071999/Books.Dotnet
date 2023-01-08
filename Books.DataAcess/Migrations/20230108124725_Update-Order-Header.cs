using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Books.DataAcess.Migrations
{
    public partial class UpdateOrderHeader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancellingDate",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletingDate",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefundingDate",
                table: "OrderHeaders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DetailOrderProcesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedProcessName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailOrderProcesses", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailOrderProcesses");

            migrationBuilder.DropColumn(
                name: "CancellingDate",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CompletingDate",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "RefundingDate",
                table: "OrderHeaders");
        }
    }
}
