using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Api.Migrations
{
    /// <inheritdoc />
    public partial class QuestionTitleMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "QuestionSubtopic");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Questions",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Answers",
                newName: "ContentText");

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ContentHtml",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentText",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubtopicId",
                table: "Questions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHtml",
                table: "Answers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_SubtopicId",
                table: "Questions",
                column: "SubtopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Subtopics_SubtopicId",
                table: "Questions",
                column: "SubtopicId",
                principalTable: "Subtopics",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Subtopics_SubtopicId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_SubtopicId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentHtml",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentText",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "SubtopicId",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "ContentHtml",
                table: "Answers");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Questions",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "ContentText",
                table: "Answers",
                newName: "Content");

            migrationBuilder.AlterColumn<int>(
                name: "TopicId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LevelId",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "QuestionSubtopic",
                columns: table => new
                {
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SubtopicId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionSubtopic", x => new { x.QuestionId, x.SubtopicId });
                    table.ForeignKey(
                        name: "FK_QuestionSubtopic_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QuestionSubtopic_Subtopics_SubtopicId",
                        column: x => x.SubtopicId,
                        principalTable: "Subtopics",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionSubtopic_SubtopicId",
                table: "QuestionSubtopic",
                column: "SubtopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
