using Microsoft.EntityFrameworkCore.Migrations;

namespace appointments.Data.Migrations
{
    public partial class UpdateRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7f217220-125d-4fec-bc36-24ce18ba1e56");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c694cb05-d030-4d21-a188-20f0d052c73f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ac143ffe-5f0f-43de-a377-eba35fdeb9ec", "de2bb3c6-5b3c-4632-aeed-a17688eef060", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "6999ce69-6a93-44ca-a5e6-5fccac25e978", "27081014-e6f0-4896-9c34-c34f3c6144ed", "AppWorker", "AppWorker" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6999ce69-6a93-44ca-a5e6-5fccac25e978");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac143ffe-5f0f-43de-a377-eba35fdeb9ec");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c694cb05-d030-4d21-a188-20f0d052c73f", "be30274c-112a-4a31-afca-6800473bbec6", "Admin", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7f217220-125d-4fec-bc36-24ce18ba1e56", "689cc248-a4fa-4334-bf7f-a23a6305a087", "AppWorker", null });
        }
    }
}
