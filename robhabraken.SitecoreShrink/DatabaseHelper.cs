
namespace robhabraken.SitecoreShrink
{
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Utility class that runs multiple queries for cleaning up a Sitecore database.
    /// </summary>
    public class DatabaseHelper
    {
        private const string BLOBS_REPORT_QUERY = @"
            DECLARE @UsableBlobs TABLE(ID UNIQUEIDENTIFIER);
            DECLARE @VersionedBlobField VARCHAR(36) = '40E50ED9-BA07-4702-992E-A912738D32DC';
            DECLARE @UnversionedBlobField VARCHAR(36) = 'DBBE7D99-1388-4357-BB34-AD71EDF18ED3';

            INSERT INTO @UsableBlobs
            SELECT CONVERT(UNIQUEIDENTIFIER, [Value]) AS EmpID
            FROM [Fields] WHERE [Value] != '' AND(FieldId = @VersionedBlobField OR FieldId = @UnversionedBlobField)

            SELECT SUM(CAST(DATALENGTH(Data) AS BIGINT)) / 1048576.0 AS usedBlobs FROM [Blobs] 
            WHERE [BlobId] IN (SELECT * FROM @UsableBlobs)

            SELECT SUM(CAST(DATALENGTH(Data) AS BIGINT)) / 1048576.0 AS unusedBlobs FROM [Blobs] 
            WHERE [BlobId] NOT IN (SELECT * FROM @UsableBlobs)
                ";

        private const string CLEAN_BLOBS_QUERY = @"
            DECLARE @UsableBlobs TABLE(ID UNIQUEIDENTIFIER);
            DECLARE @VersionedBlobField VARCHAR(36) = '40E50ED9-BA07-4702-992E-A912738D32DC';
            DECLARE @UnversionedBlobField VARCHAR(36) = 'DBBE7D99-1388-4357-BB34-AD71EDF18ED3';

            INSERT INTO @UsableBlobs
            SELECT CONVERT(UNIQUEIDENTIFIER, [Value]) AS EmpID
            FROM [Fields] WHERE [Value] != '' AND(FieldId = @VersionedBlobField OR FieldId = @UnversionedBlobField)

            DELETE FROM [Blobs] WHERE [BlobId] NOT IN(SELECT * FROM @UsableBlobs)
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

        /// <summary>
        /// Cleans up orphaned blobs (blob data of archived, recycled and deleted Sitecore media items).
        /// </summary>
        /// <remarks>
        /// If Sitecore users delete media items, Sitecore keeps the blob data to be able to restore the items later on.
        /// This method removes this obsolete data, but you should note it also invalidates archived and recycled media items!
        /// </remarks>
        public void CleanUpOrphanedBlobs()
        {
            this.ExecuteNonQuery(DatabaseHelper.CLEAN_BLOBS_QUERY);
        }

        /// <summary>
        /// Shrinks the current database. This needs admin rights and should be used with care (because index maintenance is necessary afterwards).
        /// </summary>
        /// <remarks>
        /// Calling CleanUpOrphanedBlobs can remove gigabytes of data from a very large database and this method will free up this data on disk.
        /// </remarks>
        public void ShrinkDatabase()
        {
            this.ExecuteNonQuery(DatabaseHelper.SHRINK_DATABASE_QUERY);
        }

        /// <summary>
        /// Returns a report of the current size and disk space allocation of the current database.
        /// </summary>
        /// <returns>A DatabaseReport object containing all values from the space used stored procedure.</returns>
        public void GetOrphanedBlobsSize()
        {

            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(DatabaseHelper.BLOBS_REPORT_QUERY, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.HasRows)
                    {
                        // the blobs report query only returns one row per result set
                        if (reader.Read())
                        {
                            if (reader.GetName(0).Equals("usedBlobs", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var used = reader.GetDecimal(0);
                            }
                            else if (reader.GetName(0).Equals("unusedBlobs", StringComparison.InvariantCultureIgnoreCase))
                            {
                                var unused = reader.GetDecimal(0);
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

            return;
        }

        /// <summary>
        /// Returns a report of the current size and disk space allocation of the current database.
        /// </summary>
        /// <returns>A DatabaseReport object containing all values from the space used stored procedure.</returns>
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

        /// <summary>
        /// Generic method to execute a non query on the current database, to remove duplicate code from this class.
        /// </summary>
        /// <param name="query">The (non) query to execute.</param>
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
