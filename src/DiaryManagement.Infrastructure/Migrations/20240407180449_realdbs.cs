using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DiaryManagement.Infrastructure.Migrations
{
    public partial class realdbs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaryCode",
                table: "Diarys");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaryCode",
                table: "Diarys",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
