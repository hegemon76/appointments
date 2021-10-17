using Microsoft.EntityFrameworkCore.Migrations;

namespace appointments.Data.Migrations
{
    public partial class Test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VacationStatus",
                columns: new[] { "Id", "StatusText" },
                values: new object[,]
                {
                    { 1, "Zaakceptowany" },
                    { 2, "Odrzucony" },
                    { 3, "W trakcie akceptacji" },
                    { 4, "Wymagany kontakt z przełożonym" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VacationStatus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VacationStatus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VacationStatus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VacationStatus",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
