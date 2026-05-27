using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OlimpBack.Data;

#nullable disable

namespace OlimpBack.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260519120000_RoleHierarchyLevel")]
public partial class RoleHierarchyLevel : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "Roles"
                ADD COLUMN IF NOT EXISTS "hierarchyLevel" integer NOT NULL DEFAULT 0;

            UPDATE "Roles"
            SET "hierarchyLevel" = 0
            WHERE "hierarchyLevel" IS NULL
               OR "hierarchyLevel" = 0;

            UPDATE "Roles"
            SET "hierarchyLevel" = 100
            WHERE LOWER(name) LIKE '%admin%';

            UPDATE "Roles"
            SET "hierarchyLevel" = 20
            WHERE LOWER(name) LIKE '%curator%' OR LOWER(name) LIKE '%куратор%';

            UPDATE "Roles"
            SET "hierarchyLevel" = 50
            WHERE LOWER(name) LIKE '%faculty%';
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            ALTER TABLE "Roles" DROP COLUMN IF EXISTS "hierarchyLevel";
            """);
    }
}
