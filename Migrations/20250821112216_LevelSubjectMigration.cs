using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Education.Api.Migrations
{
    /// <inheritdoc />
    public partial class LevelSubjectMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_ExamBoards_ExamBoardId",
                table: "Questions");

            migrationBuilder.DropTable(
                name: "ExamBoardSubject");

            migrationBuilder.DropTable(
                name: "SubjectTopic");

            migrationBuilder.DropIndex(
                name: "IX_Topics_Name",
                table: "Topics");

            migrationBuilder.RenameColumn(
                name: "ExamBoardId",
                table: "Questions",
                newName: "LevelId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_ExamBoardId",
                table: "Questions",
                newName: "IX_Questions_LevelId");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Topics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LevelId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Name",
                table: "Topics",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Topics_SubjectId",
                table: "Topics",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_LevelId",
                table: "Subjects",
                column: "LevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects",
                column: "LevelId",
                principalTable: "Levels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topics_Subjects_SubjectId",
                table: "Topics",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Levels_LevelId",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Levels_LevelId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Topics_Subjects_SubjectId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_Name",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Topics_SubjectId",
                table: "Topics");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_LevelId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "LevelId",
                table: "Subjects");

            migrationBuilder.RenameColumn(
                name: "LevelId",
                table: "Questions",
                newName: "ExamBoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_LevelId",
                table: "Questions",
                newName: "IX_Questions_ExamBoardId");

            migrationBuilder.CreateTable(
                name: "ExamBoardSubject",
                columns: table => new
                {
                    ExamBoardsId = table.Column<int>(type: "int", nullable: false),
                    SubjectsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamBoardSubject", x => new { x.ExamBoardsId, x.SubjectsId });
                    table.ForeignKey(
                        name: "FK_ExamBoardSubject_ExamBoards_ExamBoardsId",
                        column: x => x.ExamBoardsId,
                        principalTable: "ExamBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamBoardSubject_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectTopic",
                columns: table => new
                {
                    SubjectsId = table.Column<int>(type: "int", nullable: false),
                    TopicsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTopic", x => new { x.SubjectsId, x.TopicsId });
                    table.ForeignKey(
                        name: "FK_SubjectTopic_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectTopic_Topics_TopicsId",
                        column: x => x.TopicsId,
                        principalTable: "Topics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topics_Name",
                table: "Topics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamBoardSubject_SubjectsId",
                table: "ExamBoardSubject",
                column: "SubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectTopic_TopicsId",
                table: "SubjectTopic",
                column: "TopicsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_ExamBoards_ExamBoardId",
                table: "Questions",
                column: "ExamBoardId",
                principalTable: "ExamBoards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
