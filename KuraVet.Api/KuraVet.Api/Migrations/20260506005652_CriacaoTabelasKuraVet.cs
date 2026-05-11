using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuraVet.Api.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoTabelasKuraVet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_KV_TUTOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_KV_TUTOR", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_KV_PET",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TutorId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_KV_PET", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_KV_PET_TB_KV_TUTOR_TutorId",
                        column: x => x.TutorId,
                        principalTable: "TB_KV_TUTOR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_KV_CHECKIN_HISTORICO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataCheckin = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    FrequenciaRespiratoria = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TempoPreenchimentoCapilar = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CorMucosa = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NivelHidratacao = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    NivelRiscoIA = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_KV_CHECKIN_HISTORICO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_KV_CHECKIN_HISTORICO_TB_KV_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "TB_KV_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_KV_CHECKIN_HISTORICO_PetId",
                table: "TB_KV_CHECKIN_HISTORICO",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_TB_KV_PET_TutorId",
                table: "TB_KV_PET",
                column: "TutorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_KV_CHECKIN_HISTORICO");

            migrationBuilder.DropTable(
                name: "TB_KV_PET");

            migrationBuilder.DropTable(
                name: "TB_KV_TUTOR");
        }
    }
}
