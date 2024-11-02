using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatBot.Api.EntityFrameworkCore.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatContexts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatContexts", x => x.Id);
                    table.UniqueConstraint("AK_ChatContexts_ContextId", x => x.ContextId);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessage",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    PromptKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChatContextContextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessage_ChatContexts_ChatContextContextId",
                        column: x => x.ChatContextContextId,
                        principalTable: "ChatContexts",
                        principalColumn: "ContextId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatContexts_ContextId",
                table: "ChatContexts",
                column: "ContextId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_ChatContextContextId",
                table: "ChatMessage",
                column: "ChatContextContextId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessage_MessageId",
                table: "ChatMessage",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessage");

            migrationBuilder.DropTable(
                name: "ChatContexts");
        }
    }
}
