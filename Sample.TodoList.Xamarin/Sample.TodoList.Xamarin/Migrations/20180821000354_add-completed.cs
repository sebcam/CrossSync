using Microsoft.EntityFrameworkCore.Migrations;

namespace Sample.TodoList.Xamarin.Migrations
{
    public partial class Addcompleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "TodoLists",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "TodoLists");
        }
    }
}
