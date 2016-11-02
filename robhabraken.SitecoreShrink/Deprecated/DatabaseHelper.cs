
namespace robhabraken.SitecoreShrink.Deprecated
{
    using Sitecore;
    using Sitecore.Diagnostics;
    using Sitecore.Jobs;
    using System;
    using System.Configuration;
    using System.Data.SqlClient;

    /// <summary>
    /// Utility class that runs multiple queries for cleaning up a Sitecore database.
    /// </summary>
    /// <remarks>
    /// PLEASE NOTE that this class isn't used anymore. It works fine on itself, but I did shift the main focus of this module towards analyzing and cleaning data within Sitecore itself.
    /// Next to that, cleaning up orphans is already possible in Sitecore (via the Control Panel) and this and other features of this class also require very specific user rights that
    /// may not be available on a regular Content Management server or its database user. Lastly, the beneath methods take very long to execute on a large database, so they are better 
    /// to be executed by a system engineer than via a Sitecore module.
    /// 
    /// However, for the time being I did not yet delete these classes, but they are probably going to disappear from this module completely in the near future.
    /// </remarks>
    public class DatabaseHelper
    {
        private const string BlobsReportQuery = @"
            DECLARE @UsableBlobs TABLE(ID UNIQUEIDENTIFIER);
            DECLARE @VersionedBlobField VARCHAR(36) = '40E50ED9-BA07-4702-992E-A912738D32DC';
            DECLARE @UnversionedBlobField VARCHAR(36) = 'DBBE7D99-1388-4357-BB34-AD71EDF18ED3';

            INSERT INTO @UsableBlobs
            SELECT CONVERT(UNIQUEIDENTIFIER, [Value]) AS EmpID
            FROM [Fields] WHERE [Value] != '' AND(FieldId = @VersionedBlobField OR FieldId = @UnversionedBlobField)

            SELECT SUM(CAST(DATALENGTH(Data) AS BIGINT)) AS usedBlobs FROM [Blobs] 
            WHERE [BlobId] IN (SELECT * FROM @UsableBlobs)

            SELECT SUM(CAST(DATALENGTH(Data) AS BIGINT)) AS unusedBlobs FROM [Blobs] 
            WHERE [BlobId] NOT IN (SELECT * FROM @UsableBlobs)
                ";

        private const string CleanBlobsQuery = @"
            DECLARE @UsableBlobs TABLE(ID UNIQUEIDENTIFIER);
            DECLARE @VersionedBlobField VARCHAR(36) = '40E50ED9-BA07-4702-992E-A912738D32DC';
            DECLARE @UnversionedBlobField VARCHAR(36) = 'DBBE7D99-1388-4357-BB34-AD71EDF18ED3';

            INSERT INTO @UsableBlobs
            SELECT CONVERT(UNIQUEIDENTIFIER, [Value]) AS EmpID
            FROM [Fields] WHERE [Value] != '' AND(FieldId = @VersionedBlobField OR FieldId = @UnversionedBlobField)

            DELETE FROM [Blobs] WHERE [BlobId] NOT IN(SELECT * FROM @UsableBlobs)
                ";

        private const string ShrinkDatabaseQuery = @"
            DBCC SHRINKDATABASE(0, NOTRUNCATE)
            DBCC SHRINKDATABASE(0, TRUNCATEONLY)
                ";

        private const string SpaceUsedQuery = "EXEC sp_spaceused";

        private readonly ConnectionStringSettings connectionStringSettings;

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
            var jobName = $"{this.GetType()}_Clean_up_orphaned_media_blobs";
            var args = new object[] { CleanBlobsQuery, "cleaning up orphaned blobs" };

            var jobOptions = new JobOptions(
                jobName,
                "Database clean up",
                Context.Site.Name,
                this,
                "ExecuteNonQuery",
                args);

            JobManager.Start(jobOptions);
        }

        /// <summary>
        /// Shrinks the current database. This needs admin rights and should be used with care (because index maintenance is necessary afterwards).
        /// </summary>
        /// <remarks>
        /// Calling CleanUpOrphanedBlobs can remove gigabytes of data from a very large database and this method will free up this data on disk.
        /// </remarks>
        public void ShrinkDatabase()
        {
            this.ExecuteNonQuery(ShrinkDatabaseQuery, "shrinking the database");
        }

        /// <summary>
        /// Populates the report data regarding the blob sizes within the current database.
        /// </summary>
        /// <param name="report">The report object to populate.</param>
        public void GetOrphanedBlobsSize(ref DatabaseReport report)
        {
            if (report == null)
            {
                report = new DatabaseReport();
            }

            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(BlobsReportQuery, connection);

                try
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.HasRows)
                    {
                        // the blobs report query only returns one row per result set
                        if (reader.Read())
                        {
                            // if there is no unused data, the reader with return a SQL null value, which cannot be assigned to a decimal
                            if (!reader.IsDBNull(0))
                            {
                                // the result contains two result set, so we're detecting which result set this is by the first column name
                                if (reader.GetName(0).Equals("usedBlobs", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    report.UsedBlobsSizeInBytes = reader.GetInt64(0);
                                }
                                else if (reader.GetName(0).Equals("unusedBlobs", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    report.UnusedBlobsSizeInBytes = reader.GetInt64(0);
                                }
                            }
                        }

                        reader.NextResult();
                    }
                }
                catch (SqlException exception)
                {
                    Log.Error("Shrink: SqlException during querying for the blob sizes", exception, this);
                }
            }
        }

        /// <summary>
        /// Populates the report data regarding the current size and disk space allocation of the current database.
        /// </summary>
        /// <param name="report">The report object to populate.</param>
        public void GetSpaceUsed(ref DatabaseReport report)
        {
            if(report == null)
            {
                report = new DatabaseReport();
            }

            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(SpaceUsedQuery, connection);

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
                                report.UnusedData = reader.GetString(3);
                            }
                        }

                        reader.NextResult();
                    } 
                }
                catch (SqlException exception)
                {
                    Log.Error("Shrink: SqlException during querying for space used", exception, this);
                }
            }
        }

        /// <summary>
        /// Generic method to execute a non query on the current database, to remove duplicate code from this class.
        /// </summary>
        /// <param name="query">The (non) query to execute.</param>
        /// <param name="description">A description of what the (non) query does, used for logging if an exception might occur.</param>
        private void ExecuteNonQuery(string query, string description)
        {
            if (Context.Job != null)
            {
                Context.Job.Status.Total = 1; // indivisible task
            }

            using (var connection = new SqlConnection(this.connectionStringSettings.ConnectionString))
            {
                var command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    if (Context.Job != null)
                    {
                        Context.Job.Status.Processed++;
                    }
                }
                catch (SqlException exception)
                {
                    Log.Error($"Shrink: SqlException during {description}", exception, this);
                }
            }
        }
    }
}