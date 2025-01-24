using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShopSystem.Repository.Data.Migrations.Identity
{
    /// <inheritdoc />
    public partial class DateinLogin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1cf380a3-25da-4bd6-99d5-6ffcd179321f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62d015d5-bb79-40f0-8c59-4466def35cc1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aa929c19-28bf-4840-b6cf-de3e62be7d79");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ed0c50a0-6148-4a70-90af-79b3f47efbaf");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "DurationSpent",
                table: "AspNetUsers",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "38f07da3-7bf3-4759-8a3c-69022b34e8d0", "1", "User", "USER" },
                    { "4523fc22-a4ea-4b8f-8cc4-3678828e7a66", "4", "Admin", "ADMIN" },
                    { "89c41325-8d54-4c94-8c14-e6b772988567", "3", "ServiceProvider", "SERVICEPROVIDER" },
                    { "ac21fa2f-fdda-41cc-ad97-d12003ca7c04", "2", "BussinesOwner", "BUSSINESOWNER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "38f07da3-7bf3-4759-8a3c-69022b34e8d0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4523fc22-a4ea-4b8f-8cc4-3678828e7a66");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "89c41325-8d54-4c94-8c14-e6b772988567");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ac21fa2f-fdda-41cc-ad97-d12003ca7c04");

            migrationBuilder.DropColumn(
                name: "DurationSpent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LoginTime",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LogoutTime",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1cf380a3-25da-4bd6-99d5-6ffcd179321f", "3", "ServiceProvider", "SERVICEPROVIDER" },
                    { "62d015d5-bb79-40f0-8c59-4466def35cc1", "4", "Admin", "ADMIN" },
                    { "aa929c19-28bf-4840-b6cf-de3e62be7d79", "2", "BussinesOwner", "BUSSINESOWNER" },
                    { "ed0c50a0-6148-4a70-90af-79b3f47efbaf", "1", "User", "USER" }
                });
        }
    }
}
