using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Infrastructure.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prompts_Owner_Key",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "ChatContexts");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Prompts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ChatContexts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_OwnerId_Key",
                table: "Prompts",
                columns: new[] { "OwnerId", "Key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Prompts_OwnerId_Key",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Prompts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ChatContexts");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "Prompts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "ChatContexts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_Owner_Key",
                table: "Prompts",
                columns: new[] { "Owner", "Key" },
                unique: true);
        }
    }
}
