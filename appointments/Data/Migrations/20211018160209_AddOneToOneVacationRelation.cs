using Microsoft.EntityFrameworkCore.Migrations;

namespace appointments.Data.Migrations
{
    public partial class AddOneToOneVacationRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "VacationStatusId",
                table: "Vacations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Vacations_VacationStatusId",
                table: "Vacations",
                column: "VacationStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacations_VacationStatus_VacationStatusId",
                table: "Vacations",
                column: "VacationStatusId",
                principalTable: "VacationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vacations_VacationStatus_VacationStatusId",
                table: "Vacations");

            migrationBuilder.DropIndex(
                name: "IX_Vacations_VacationStatusId",
                table: "Vacations");

            migrationBuilder.AlterColumn<int>(
                name: "VacationStatusId",
                table: "Vacations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
