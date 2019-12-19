using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendCapstone.Migrations
{
    public partial class EventDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Events",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "3564e8ef-2c89-4172-98de-31e0ff006c65", "AQAAAAEAACcQAAAAEG6c3Ja/joTv29h/R8b0aSi2GI1sfcQsR3kSyGmZGEyeBj+v9RXp6T2AvLx2BMi+9g==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "04ce3b02-c4ea-4091-98ff-bfa7f77f10fb", "AQAAAAEAACcQAAAAEKPssKx4Qw0l8ghDT0V4DQoDxxGiqebCJF/bfF4Ut3Pz+Wmzp54qRei/zDgJGlX/nw==" });
        }
    }
}
