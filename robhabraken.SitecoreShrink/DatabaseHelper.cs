
namespace robhabraken.SitecoreShrink
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Configuration;

    public class DatabaseHelper
    {
        private const string CLEAN_BLOBS_QUERY = @"
            DECLARE @UsableBlobs TABLE(ID UNIQUEIDENTIFIER);
            DECLARE @VersionedBlobField VARCHAR(36) = '40E50ED9-BA07-4702-992E-A912738D32DC';
            DECLARE @UnversionedBlobField VARCHAR(36) = 'DBBE7D99-1388-4357-BB34-AD71EDF18ED3';

            INSERT INTO
                @UsableBlobs
            SELECT
                CONVERT(UNIQUEIDENTIFIER, [Value]) AS EmpID
            FROM
                [Fields]
            WHERE
                [Value] != '' AND(FieldId = @VersionedBlobField OR FieldId = @UnversionedBlobField)

            DELETE FROM
                [Blobs]
            WHERE
                [BlobId] NOT IN(SELECT * FROM @UsableBlobs)
                ";

        private const string SHRINK_DATABASE_QUERY = @"
            DBCC SHRINKDATABASE(0, NOTRUNCATE)
            DBCC SHRINKDATABASE(0, TRUNCATEONLY)
                ";

        private const string SPACE_USED_QUERY = "EXEC sp_spaceused";

        private ConnectionStringSettings connectionStringSettings;

        public DatabaseHelper(string databaseName)
        {
            this.connectionStringSettings = ConfigurationManager.ConnectionStrings[databaseName];
        }

        public void CleanUpOrphanedBlobs()
        {
            this.ExecuteNonQuery(DatabaseHelper.CLEAN_BLOBS_QUERY);
        }   

        public void ShrinkDatabase()
        {
            this.ExecuteNonQuery(DatabaseHelper.SHRINK_DATABASE_QUERY);
        }

        public DatabaseReport GetSpaceUsed()
        {
            DatabaseReport report = null;

            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(DatabaseHelper.SPACE_USED_QUERY, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    report = new DatabaseReport();
                    while (reader.HasRows)
                    {
                        // the space used stored procedure only returns one row per result set
                        if (reader.Read())
                        {
                            if (reader.GetName(0).Equals("database_name", StringComparison.InvariantCultureIgnoreCase) && reader.FieldCount >= 3)
                            {
                                report.DatabaseName = reader.GetString(0);
                                report.DatabaseSize = reader.GetString(1);
                                report.UnallocatedSpace = reader.GetString(2);
                            }
                            else if (reader.GetName(0).Equals("reserved", StringComparison.InvariantCultureIgnoreCase) && reader.FieldCount >= 4)
                            {
                                report.Reserved = reader.GetString(0);
                                report.Data = reader.GetString(1);
                                report.IndexSize = reader.GetString(2);
                                report.Unused = reader.GetString(3);
                            }
                        }

                        reader.NextResult();
                    } 
                }
                catch (SqlException exception)
                {
                    var x = exception.Message;
                }
            }

            return report;
        }

        private void ExecuteNonQuery(string query)
        {
            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException exception)
                {
                    var x = exception.Message;
                }
            }
        }
    }
}
