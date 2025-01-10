using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Demo.DbValuesChangeMonitoring.Data.Migrations
{
    /// <inheritdoc />
    public partial class QueueSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
				"""
                ALTER DATABASE ValuesChangedMonitoring SET ENABLE_BROKER;                
                """,
                suppressTransaction: true
				);

			migrationBuilder.Sql(
				"""
                CREATE QUEUE ValuesChangeEventQueue;
                CREATE SERVICE ValuesChangeEventService ON QUEUE ValuesChangeEventQueue ([DEFAULT]);               
                """
				);

			migrationBuilder.Sql(
				"""
                CREATE TRIGGER trg_ConfigurationValues_InsertUpdate
                ON configuration.ConfigurationValues
                AFTER INSERT, UPDATE, DELETE
                AS
                BEGIN
                    DECLARE @handle uniqueidentifier
                    BEGIN DIALOG CONVERSATION @handle 
                    FROM SERVICE [ValuesChangeEventService]
                    TO SERVICE 'ValuesChangeEventService'
                    ON CONTRACT [DEFAULT]
                    WITH ENCRYPTION = OFF;

                    SEND ON CONVERSATION @handle
                    MESSAGE TYPE [DEFAULT]
                    (N'configuration.ConfigurationValues');
                END;
                              
                """
				);
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(
				"""
                DROP TRIGGER IF EXISTS trg_ConfigurationValues_InsertUpdate ON ConfigurationValues;                     
                """
				);

			migrationBuilder.Sql(
				"""
                DROP SERVICE IF EXISTS ValuesChangeEventService;                       
                """
				);

			migrationBuilder.Sql(
				"""
                DROP QUEUE IF EXISTS ValuesChangeEventQueue;                      
                """
				);

			migrationBuilder.Sql(
				"""
                ALTER DATABASE YourDatabase SET ENABLE_BROKER;                      
                """
				);
		}
    }
}
