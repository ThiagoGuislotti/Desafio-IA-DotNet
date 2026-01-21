using Microsoft.EntityFrameworkCore.Migrations;

namespace CustomerPlatform.Infrastructure.Data.Context.Migrations
{
    /// <summary>
    /// Migration inicial da base de dados.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    customerId = table.Column<Guid>(type: "uuid", nullable: false),
                    customerType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    fullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    corporateName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tradeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    document = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    birthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    email = table.Column<string>(type: "character varying(254)", maxLength: 254, nullable: false),
                    phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    addressStreet = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    addressNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    addressComplement = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    addressPostalCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    addressCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    addressState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_Customers", x => x.customerId);
                });

            migrationBuilder.CreateTable(
                name: "DuplicateSuspicions",
                columns: table => new
                {
                    duplicateSuspicionId = table.Column<Guid>(type: "uuid", nullable: false),
                    customerId = table.Column<Guid>(type: "uuid", nullable: false),
                    candidateCustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_DuplicateSuspicions", x => x.duplicateSuspicionId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxEvents",
                columns: table => new
                {
                    outboxEventId = table.Column<Guid>(type: "uuid", nullable: false),
                    eventId = table.Column<Guid>(type: "uuid", nullable: false),
                    eventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retryCount = table.Column<int>(type: "integer", nullable: false),
                    lastError = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_OutboxEvents", x => x.outboxEventId);
                });

            migrationBuilder.CreateIndex(
                name: "ix_Customers_document",
                table: "Customers",
                column: "document",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_DuplicateSuspicions_candidateCustomerId",
                table: "DuplicateSuspicions",
                column: "candidateCustomerId");

            migrationBuilder.CreateIndex(
                name: "ix_DuplicateSuspicions_customerId",
                table: "DuplicateSuspicions",
                column: "customerId");

            migrationBuilder.CreateIndex(
                name: "ix_OutboxEvents_eventId",
                table: "OutboxEvents",
                column: "eventId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DuplicateSuspicions");

            migrationBuilder.DropTable(
                name: "OutboxEvents");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}