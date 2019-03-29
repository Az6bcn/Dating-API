using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace DatingAPI.Migrations
{
    public partial class ChangedPhotoCloudinaryPropertyDataTypeToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CloudinaryID",
                table: "Photos",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CloudinaryID",
                table: "Photos",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
