using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AlwaysEncrypted.Entities.Migrations
{
    public partial class Ado_Linq_Patients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdoPatients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),
                    Ssn = table.Column<string>(type: "char(11)", maxLength: 11, nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoPatients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LinqPatients",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false),

                    //  Strategy, disable and set manually to the preferred data type.
                    //Ssn = table.Column<string>(maxLength: 11, nullable: false),

                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),

                    //  Strategy, disable and set manually to the preferred data type.
                    //BirthDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinqPatients", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoPatients_FirstName",
                table: "AdoPatients",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_AdoPatients_LastName",
                table: "AdoPatients",
                column: "LastName");

            migrationBuilder.CreateIndex(
                name: "IX_AdoPatients_Ssn",
                table: "AdoPatients",
                column: "Ssn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LinqPatients_FirstName",
                table: "LinqPatients",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_LinqPatients_LastName",
                table: "LinqPatients",
                column: "LastName");

            //  Alter Patients table adding encrypted Ssn column.
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[LinqPatients]
                ADD		[Ssn] [NVARCHAR](11) COLLATE Latin1_General_BIN2 
			                ENCRYPTED WITH (ENCRYPTION_TYPE = DETERMINISTIC
			                , ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256'
			                , COLUMN_ENCRYPTION_KEY = PatientCEK) NOT NULL;
            ");

            //  Alter Patients table adding encrypted BirthDate column.
            migrationBuilder.Sql(@"
                ALTER TABLE [dbo].[LinqPatients]
                ADD		[BirthDate] [DATETIME2]
			                ENCRYPTED WITH (ENCRYPTION_TYPE = RANDOMIZED
			                , ALGORITHM = 'AEAD_AES_256_CBC_HMAC_SHA_256'
			                , COLUMN_ENCRYPTION_KEY = PatientCEK) NOT NULL;
            ");

            //  Create Unique Index on column Ssn - includes delete if exists.
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT	[name] 
			            FROM	sys.indexes  
                       WHERE	[name] = N'IX_LinqPatients_Ssn_Unique')   
                    DROP INDEX IX_LinqPatients_Ssn_Unique ON [dbo].[LinqPatients];  
            ");
            migrationBuilder.Sql(@"
                CREATE UNIQUE INDEX IX_LinqPatients_Ssn_Unique   
                    ON [dbo].[LinqPatients] ([Ssn]);   
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT	[name] 
			                FROM	sys.indexes  
                            WHERE	[name] = N'IX_LinqPatients_Ssn_Unique')   
                    DROP INDEX IX_LinqPatients_Ssn_Unique ON [dbo].[LinqPatients];  
            ");
            migrationBuilder.DropTable(
                name: "AdoPatients");

            migrationBuilder.DropTable(
                name: "LinqPatients");
        }
    }
}
