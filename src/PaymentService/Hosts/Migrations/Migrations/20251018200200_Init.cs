using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Hosts.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "PaymentStore",
                columns: table => new
                {
                    AggregateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", maxLength: 200, nullable: false),
                    Payload = table.Column<string>(type: "json", nullable: false),
                    Metadata = table.Column<string>(type: "json", nullable: false, defaultValue: "{}"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentStore", x => new { x.AggregateId, x.Version });
                    table.CheckConstraint("CK_PaymentStore_Version_NonNegative", "\"Version\" >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStore_AggregateId_Version",
                table: "PaymentStore",
                columns: new[] { "AggregateId", "Version" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentStore_EventId",
                table: "PaymentStore",
                column: "EventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentStore");
        }
    }
}
