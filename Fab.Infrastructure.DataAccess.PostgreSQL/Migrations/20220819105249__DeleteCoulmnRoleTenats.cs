using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fab.Infrastructure.DataAccess.PostgreSQL.Migrations
{
    public partial class _DeleteCoulmnRoleTenats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantsRole",
                table: "Tenants");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TenantsRole",
                table: "Tenants",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
