using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuraVet.Infrastructure.Persistence.Migrations
{
    /// Sprint 4: tabela de usuários para autenticação JWT (senha armazenada com hash BCrypt).
    public partial class AdicionaUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_KV_USUARIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Perfil = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_KV_USUARIO", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_KV_USUARIO_Email",
                table: "TB_KV_USUARIO",
                column: "Email",
                unique: true);
        }

  
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_KV_USUARIO");
        }
    }
}
