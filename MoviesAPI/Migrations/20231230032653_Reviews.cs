﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations;

/// <inheritdoc />
public partial class Reviews : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "Reviews",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Comentario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Puntuacion = table.Column<int>(type: "int", nullable: false),
                MovieId = table.Column<int>(type: "int", nullable: false),
                UsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Reviews", x => x.Id);
                table.ForeignKey(
                    name: "FK_Reviews_AspNetUsers_UsuarioId",
                    column: x => x.UsuarioId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Reviews_Movies_MovieId",
                    column: x => x.MovieId,
                    principalTable: "Movies",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Reviews_MovieId",
            table: "Reviews",
            column: "MovieId");

        migrationBuilder.CreateIndex(
            name: "IX_Reviews_UsuarioId",
            table: "Reviews",
            column: "UsuarioId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "Reviews");
    }
}