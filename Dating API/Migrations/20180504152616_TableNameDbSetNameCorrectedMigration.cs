using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DatingAPI.Migrations
{
    public partial class TableNameDbSetNameCorrectedMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Valuesalues",
                table: "Valuesalues");

            migrationBuilder.RenameTable(
                name: "Valuesalues",
                newName: "Values");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Values",
                table: "Values",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Values",
                table: "Values");

            migrationBuilder.RenameTable(
                name: "Values",
                newName: "Valuesalues");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Valuesalues",
                table: "Valuesalues",
                column: "Id");
        }
    }
}
