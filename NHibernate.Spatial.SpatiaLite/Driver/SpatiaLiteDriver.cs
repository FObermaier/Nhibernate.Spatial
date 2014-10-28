using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using NHibernate.Driver;
using log4net;
using Environment = System.Environment;

namespace NHibernate.Spatial.Driver
{
    public class SpatiaLiteDriver : SQLite20Driver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SpatiaLiteDriver));

        static SpatiaLiteDriver()
        {
            var path = Environment.GetEnvironmentVariable("PATH");
            if (path == null)
                throw new Exception();

            var directory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            if (string.IsNullOrEmpty(directory))
                throw new Exception();

            var spatiaLitePath = Path.Combine(directory, IntPtr.Size == 4 ? "x86" : "x64");
            if (string.IsNullOrEmpty(spatiaLitePath))
            {
                Log.Info("SpatiaLite path not configured.");
            }
            else
            {
                Log.Info(string.Format("SpatiaLite configured for '{0}'.", spatiaLitePath));
                if (!Directory.Exists(spatiaLitePath))
                    throw new DirectoryNotFoundException(spatiaLitePath);

                if (!path.ToLower().Contains(spatiaLitePath))
                    Environment.SetEnvironmentVariable("PATH", spatiaLitePath + ";" + path);
            }
        }
        public override IDbConnection CreateConnection()
        {
            var cn = (DbConnection)base.CreateConnection();
            cn.StateChange += ConnectionStateChangeHandler;
            return cn;
        }


        private static void ConnectionStateChangeHandler(object sender, StateChangeEventArgs e)
        {
            if (((e.OriginalState != ConnectionState.Broken && e.OriginalState != ConnectionState.Closed) &&
                 e.OriginalState != ConnectionState.Connecting) || e.CurrentState != ConnectionState.Open) return;
            
            var connection = (DbConnection)sender;
            using (var command = connection.CreateCommand())
            {
                // Activated foreign keys if supported by SQLite.  Unknown pragmas are ignored.
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();

                var path = "spatialite.dll";
                command.CommandText = string.Format("SELECT load_extension('{0}');", path);
                command.ExecuteNonQuery();

                command.CommandText = @"SELECT InitSpatialMetadata('NONE') WHERE CheckSpatialMetadata()=0;";
                command.ExecuteNonQuery();
                
                ////Maybe add srs_wkt
                //try
                //{
                //    command.CommandText = "SELECT \"sql\" FROM \"sqlite_master\" WHERE \"name\"='spatial_ref_sys';";
                //    var res = command.ExecuteScalar();
                //    if (res == DBNull.Value || ((string)res).IndexOf("srs_wkt") == -1)
                //    {
                //        //command.CommandText = "SELECT \"srs_wkt\" FROM \"spatial_ref_sys\" LIMIT 1;";
                //        //var result = command.ExecuteScalar();
                //        command.CommandText = "ALTER TABLE spatial_ref_sys ADD COLUMN \"srs_wkt\" TEXT;";
                //        command.ExecuteNonQuery();
                //    }
                //}
                //catch (Exception)
                //{
                //}
            }
        }

    }
}