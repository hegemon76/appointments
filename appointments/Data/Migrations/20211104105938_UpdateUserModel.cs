using Microsoft.EntityFrameworkCore.Migrations;

namespace appointments.Data.Migrations
{
    public partial class UpdateUserModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6999ce69-6a93-44ca-a5e6-5fccac25e978");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac143ffe-5f0f-43de-a377-eba35fdeb9ec");

            migrationBuilder.AddColumn<int>(
                name: "VacationDays",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VacationDaysTaken",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b7b29c78-e4b1-4800-bf82-3551c3b01988", "b3652416-7fe5-466b-b739-e2a6a9b1fd4d", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "dbf1618e-4fd3-46e0-a090-cea252ac3a1e", "37d1eea2-0bb3-491b-bfa5-839d12e71eb0", "AppWorker", "AppWorker" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b7b29c78-e4b1-4800-bf82-3551c3b01988");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "dbf1618e-4fd3-46e0-a090-cea252ac3a1e");

            migrationBuilder.DropColumn(
                name: "VacationDays",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "VacationDaysTaken",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ac143ffe-5f0f-43de-a377-eba35fdeb9ec", "de2bb3c6-5b3c-4632-aeed-a17688eef060", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6999ce69-6a93-44ca-a5e6-5fccac25e978", "27081014-e6f0-4896-9c34-c34f3c6144ed", "AppWorker", "AppWorker" });
        }
    }
}
