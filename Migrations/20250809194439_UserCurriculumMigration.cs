using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserCurriculumMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExamBoardId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LevelUser",
                columns: table => new
                {
                    LevelsId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LevelUser", x => new { x.LevelsId, x.UserId });
                    table.ForeignKey(
                        name: "FK_LevelUser_Levels_LevelsId",
                        column: x => x.LevelsId,
                        principalTable: "Levels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LevelUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurriculumId",
                table: "Users",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ExamBoardId",
                table: "Users",
                column: "ExamBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LevelUser_UserId",
                table: "LevelUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Curriculums_CurriculumId",
                table: "Users",
                column: "CurriculumId",
                principalTable: "Curriculums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_ExamBoards_ExamBoardId",
                table: "Users",
                column: "ExamBoardId",
                principalTable: "ExamBoards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Curriculums_CurriculumId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_ExamBoards_ExamBoardId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "LevelUser");

            migrationBuilder.DropIndex(
                name: "IX_Users_CurriculumId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ExamBoardId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExamBoardId",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
