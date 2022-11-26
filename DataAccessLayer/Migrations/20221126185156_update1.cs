using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileData_AspNetUsers_AppUserId",
                schema: "dbo",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_FileData_File_AppFileId",
                schema: "dbo",
                table: "FileData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_File",
                schema: "dbo",
                table: "File");

            migrationBuilder.RenameTable(
                name: "File",
                schema: "dbo",
                newName: "Files",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                schema: "dbo",
                table: "FileData",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_FileData_AppUserId",
                schema: "dbo",
                table: "FileData",
                newName: "IX_FileData_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ShortLink",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UnstrustedName",
                schema: "dbo",
                table: "FileData",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "dbo",
                table: "FileData",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                schema: "dbo",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ShortLink_Link",
                table: "ShortLink",
                column: "Link",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_Files_AppFileId",
                schema: "dbo",
                table: "FileData",
                column: "AppFileId",
                principalSchema: "dbo",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_OwnerId",
                schema: "dbo",
                table: "FileData",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileData_Files_AppFileId",
                schema: "dbo",
                table: "FileData");

            migrationBuilder.DropForeignKey(
                name: "FK_FileData_OwnerId",
                schema: "dbo",
                table: "FileData");

            migrationBuilder.DropIndex(
                name: "IX_ShortLink_Link",
                table: "ShortLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                schema: "dbo",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                schema: "dbo",
                newName: "File",
                newSchema: "dbo");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "dbo",
                table: "FileData",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FileData_OwnerId",
                schema: "dbo",
                table: "FileData",
                newName: "IX_FileData_AppUserId");

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "ShortLink",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<string>(
                name: "UnstrustedName",
                schema: "dbo",
                table: "FileData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Note",
                schema: "dbo",
                table: "FileData",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_File",
                schema: "dbo",
                table: "File",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_AspNetUsers_AppUserId",
                schema: "dbo",
                table: "FileData",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FileData_File_AppFileId",
                schema: "dbo",
                table: "FileData",
                column: "AppFileId",
                principalSchema: "dbo",
                principalTable: "File",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
