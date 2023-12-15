using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LoncotesLibrary.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MaterialTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CheckoutDays = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patrons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patrons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialName = table.Column<string>(type: "text", nullable: false),
                    MaterialTypeId = table.Column<int>(type: "integer", nullable: false),
                    GenreId = table.Column<int>(type: "integer", nullable: false),
                    OutOfCirculationSince = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Materials_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Materials_MaterialTypes_MaterialTypeId",
                        column: x => x.MaterialTypeId,
                        principalTable: "MaterialTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Checkouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaterialId = table.Column<int>(type: "integer", nullable: false),
                    PatronId = table.Column<int>(type: "integer", nullable: false),
                    CheckoutDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Paid = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkouts_Materials_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materials",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Checkouts_Patrons_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patrons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Fiction" },
                    { 2, "Science Fiction" },
                    { 3, "History" },
                    { 4, "Nonfiction" },
                    { 5, "Mystery" }
                });

            migrationBuilder.InsertData(
                table: "MaterialTypes",
                columns: new[] { "Id", "CheckoutDays", "Name" },
                values: new object[,]
                {
                    { 1, 30, "Book" },
                    { 2, 14, "Magazine" },
                    { 3, 7, "DVD" },
                    { 4, 21, "Audio Book" },
                    { 5, 14, "E-book" }
                });

            migrationBuilder.InsertData(
                table: "Patrons",
                columns: new[] { "Id", "Address", "Email", "FirstName", "IsActive", "LastName" },
                values: new object[,]
                {
                    { 1, "4199 High Street", "lalecksen@gmail.com", "Lukas", true, "Alecksen" },
                    { 2, "123 Main Street", "asmith@gmail.com", "Anna", true, "Smith" },
                    { 3, "456 Oak Avenue", "jdoe@gmail.com", "John", true, "Doe" },
                    { 4, "789 Pine Lane", "ejohnson@gmail.com", "Emily", true, "Johnson" },
                    { 5, "101 Cedar Road", "dbrown@gmail.com", "Daniel", true, "Brown" },
                    { 6, "202 Birch Street", "omiller@gmail.com", "Olivia", true, "Miller" },
                    { 7, "303 Maple Avenue", "mwhite@gmail.com", "Michael", true, "White" },
                    { 8, "404 Elm Boulevard", "sdavis@gmail.com", "Sophia", true, "Davis" },
                    { 9, "505 Oak Drive", "eanderson@gmail.com", "Ethan", false, "Anderson" },
                    { 10, "606 Pine Circle", "amartinez@gmail.com", "Ava", false, "Martinez" }
                });

            migrationBuilder.InsertData(
                table: "Materials",
                columns: new[] { "Id", "GenreId", "MaterialName", "MaterialTypeId", "OutOfCirculationSince" },
                values: new object[,]
                {
                    { 1, 1, "The Great Gatsby", 1, null },
                    { 2, 4, "National Geographic", 2, null },
                    { 3, 2, "Inception (DVD)", 3, null },
                    { 4, 3, "The Brief History of Time", 4, null },
                    { 5, 5, "The Da Vinci Code (E-book)", 5, null },
                    { 6, 3, "The Art of War", 1, null },
                    { 7, 4, "Time Magazine", 2, null },
                    { 8, 2, "The Matrix (DVD)", 3, null },
                    { 9, 3, "Sapiens (Audio Book)", 4, null },
                    { 10, 2, "Neuromancer (E-book)", 5, null },
                    { 11, 4, "National Geographic Kids", 2, null },
                    { 12, 1, "Spider-Man: Into the Spider-Verse (DVD)", 3, null },
                    { 13, 3, "A Brief History of Nearly Everything", 4, null },
                    { 14, 2, "The Hitchhiker's Guide to the Galaxy (E-book)", 5, null },
                    { 15, 5, "Cook's Illustrated", 4, null },
                    { 16, 1, "Black Mirror (DVD)", 3, null },
                    { 17, 1, "The Catcher in the Rye", 1, new DateTime(2011, 12, 30, 1, 1, 1, 0, DateTimeKind.Unspecified) },
                    { 18, 4, "Wired Magazine", 2, null },
                    { 19, 5, "The Shining (DVD)", 3, new DateTime(1999, 9, 22, 1, 1, 1, 0, DateTimeKind.Unspecified) },
                    { 20, 5, "The Girl with the Dragon Tattoo (E-book)", 5, null }
                });

            migrationBuilder.InsertData(
                table: "Checkouts",
                columns: new[] { "Id", "CheckoutDate", "MaterialId", "Paid", "PatronId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 15, 12, 30, 0, 0, DateTimeKind.Unspecified), 7, true, 1, new DateTime(2023, 2, 28, 14, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2023, 2, 20, 10, 15, 30, 0, DateTimeKind.Unspecified), 11, true, 4, new DateTime(2023, 3, 5, 11, 20, 45, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2023, 3, 10, 16, 5, 22, 0, DateTimeKind.Unspecified), 14, false, 7, new DateTime(2023, 5, 24, 18, 30, 11, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2023, 4, 5, 14, 0, 15, 0, DateTimeKind.Unspecified), 17, false, 9, new DateTime(2023, 4, 19, 16, 10, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2023, 12, 1, 9, 45, 30, 0, DateTimeKind.Unspecified), 20, true, 10, new DateTime(2023, 12, 3, 10, 10, 10, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2023, 12, 12, 14, 30, 0, 0, DateTimeKind.Unspecified), 8, true, 2, null },
                    { 7, new DateTime(2023, 12, 13, 10, 33, 11, 0, DateTimeKind.Unspecified), 13, true, 1, null },
                    { 8, new DateTime(2023, 12, 13, 16, 42, 11, 0, DateTimeKind.Unspecified), 12, true, 6, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_MaterialId",
                table: "Checkouts",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkouts_PatronId",
                table: "Checkouts",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_GenreId",
                table: "Materials",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_MaterialTypeId",
                table: "Materials",
                column: "MaterialTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checkouts");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Patrons");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "MaterialTypes");
        }
    }
}
