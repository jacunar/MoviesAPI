﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAPI.Migrations;

/// <inheritdoc />
public partial class Movies : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "Movies",
            columns: table => new {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                OnCimena = table.Column<bool>(type: "bit", nullable: false),
                Premiere = table.Column<DateTime>(type: "datetime2", nullable: false),
                Poster = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_Movies", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable(
            name: "Movies");
    }
}