using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionBookmarkMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionBookmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBookmarks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionBookmarks_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionBookmarks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBookmarks_QuestionId",
                table: "QuestionBookmarks",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionBookmarks_UserId",
                table: "QuestionBookmarks",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestionBookmarks");
        }
    }
}
