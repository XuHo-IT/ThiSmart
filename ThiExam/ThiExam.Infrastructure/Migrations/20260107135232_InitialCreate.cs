using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThiExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__roles__3213E83F572C2184", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__3213E83FA4AE5AF3", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    parent_id = table.Column<int>(type: "int", nullable: true),
                    teacher_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__3213E83FC9886B27", x => x.id);
                    table.ForeignKey(
                        name: "FK_categories_parent",
                        column: x => x.parent_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_categories_teacher",
                        column: x => x.teacher_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "exams",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    teacher_id = table.Column<int>(type: "int", nullable: true),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: false),
                    pass_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    is_random = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    snapshot_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    anti_cheat_settings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__exams__3213E83F361A29AA", x => x.id);
                    table.ForeignKey(
                        name: "FK_exams_teacher",
                        column: x => x.teacher_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    teacher_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    level = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    question_type = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "MultipleChoice"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__3213E83F9C6A1D79", x => x.id);
                    table.ForeignKey(
                        name: "FK_questions_category",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_questions_teacher",
                        column: x => x.teacher_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "exam_sessions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    exam_id = table.Column<int>(type: "int", nullable: true),
                    access_code = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_active = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__exam_ses__3213E83FD397FDEA", x => x.id);
                    table.ForeignKey(
                        name: "FK_sessions_exam",
                        column: x => x.exam_id,
                        principalTable: "exams",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "exam_questions",
                columns: table => new
                {
                    exam_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    order_index = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__exam_que__1E605ABDADC18F90", x => new { x.exam_id, x.question_id });
                    table.ForeignKey(
                        name: "FK_exam_questions_exam",
                        column: x => x.exam_id,
                        principalTable: "exams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_exam_questions_question",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "question_options",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_correct = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__3213E83FCCB8D8B4", x => x.id);
                    table.ForeignKey(
                        name: "FK_options_question",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "student_attempts",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    session_id = table.Column<int>(type: "int", nullable: true),
                    student_id = table.Column<int>(type: "int", nullable: true),
                    started_at = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    total_score = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true, defaultValue: "In-Progress")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__student___3213E83F3E6CFBF9", x => x.id);
                    table.ForeignKey(
                        name: "FK_attempts_session",
                        column: x => x.session_id,
                        principalTable: "exam_sessions",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_attempts_student",
                        column: x => x.student_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "proctoring_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    attempt_id = table.Column<int>(type: "int", nullable: true),
                    event_type = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    log_time = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(sysdatetime())"),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__proctori__3213E83F6795B251", x => x.id);
                    table.ForeignKey(
                        name: "FK_logs_attempt",
                        column: x => x.attempt_id,
                        principalTable: "student_attempts",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "student_answers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    attempt_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: true),
                    selected_option_id = table.Column<int>(type: "int", nullable: true),
                    essay_answer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_flagged = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__student___3213E83FCEA8D736", x => x.id);
                    table.ForeignKey(
                        name: "FK_answers_attempt",
                        column: x => x.attempt_id,
                        principalTable: "student_attempts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_answers_option",
                        column: x => x.selected_option_id,
                        principalTable: "question_options",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_answers_question",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_parent_id",
                table: "categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_categories_teacher_id",
                table: "categories",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "IX_exam_questions_question_id",
                table: "exam_questions",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "idx_exam_access_code",
                table: "exam_sessions",
                column: "access_code");

            migrationBuilder.CreateIndex(
                name: "IX_exam_sessions_exam_id",
                table: "exam_sessions",
                column: "exam_id");

            migrationBuilder.CreateIndex(
                name: "UQ__exam_ses__D304327196FD30D6",
                table: "exam_sessions",
                column: "access_code",
                unique: true,
                filter: "[access_code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_exams_teacher_id",
                table: "exams",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "idx_proctoring_attempt",
                table: "proctoring_logs",
                column: "attempt_id");

            migrationBuilder.CreateIndex(
                name: "IX_question_options_question_id",
                table: "question_options",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "idx_question_category",
                table: "questions",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_questions_teacher_id",
                table: "questions",
                column: "teacher_id");

            migrationBuilder.CreateIndex(
                name: "UQ__roles__783254B1775C2742",
                table: "roles",
                column: "role_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_student_answers_attempt_id",
                table: "student_answers",
                column: "attempt_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_answers_question_id",
                table: "student_answers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_answers_selected_option_id",
                table: "student_answers",
                column: "selected_option_id");

            migrationBuilder.CreateIndex(
                name: "idx_student_attempt_session",
                table: "student_attempts",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "IX_student_attempts_student_id",
                table: "student_attempts",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "UQ__users__AB6E6164CB76162D",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__users__F3DBC572CA51E503",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exam_questions");

            migrationBuilder.DropTable(
                name: "proctoring_logs");

            migrationBuilder.DropTable(
                name: "student_answers");

            migrationBuilder.DropTable(
                name: "student_attempts");

            migrationBuilder.DropTable(
                name: "question_options");

            migrationBuilder.DropTable(
                name: "exam_sessions");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "exams");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
