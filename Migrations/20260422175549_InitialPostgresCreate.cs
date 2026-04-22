using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace OlimpBack.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // УВАГА: Метод Up залишаємо порожнім!
            // Оскільки твої робочі таблиці вже існують у PostgreSQL,
            // нам не потрібно, щоб EF намагався створити їх заново.
            // Ця порожня міграція просто "позначить" для EF, що база готова.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Метод Down теж порожній.
            // Ми не хочемо випадково видалити твої робочі дані при відкаті міграції.
        }
    }
}