using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuraVet.Api.Migrations
{
    /// <inheritdoc />
    public partial class AdicionaEventoConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_KV_EVENTO_CONSULTA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    TipoEvento = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    DataEvento = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    Descricao = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    VeterinarioResponsavel = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    PetId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_KV_EVENTO_CONSULTA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_KV_EVENTO_CONSULTA_TB_KV_PET_PetId",
                        column: x => x.PetId,
                        principalTable: "TB_KV_PET",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_KV_EVENTO_CONSULTA_PetId",
                table: "TB_KV_EVENTO_CONSULTA",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_KV_EVENTO_CONSULTA");
        }
    }
}
