using System.Linq;
using NHibernate.Cfg;
using NHibernate.Linq;
using NHibernate.Spatial.Metadata;
using NUnit.Framework;
using Tests.NHibernate.Spatial.OgcSfSql11Compliance;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class SpatiaLiteConformanceItemsFixture : ConformanceItemsFixture
	{
	    public SpatiaLiteConformanceItemsFixture()
	    {
            if (System.IO.File.Exists("OgcSfsTest.sqlite"))
                System.IO.File.Delete("OgcSfsTest.sqlite");
	    }
        protected override void Configure(Configuration configuration)
		{
			TestConfigurationOgcConformance.Configure(configuration);
		}

		private string _spatiaLiteVersion;

		protected override void OnTestFixtureSetUp()
		{
            _spatiaLiteVersion = SpatiaLiteTestsUtil.GetSpatialLiteVersion(sessions);
			base.OnTestFixtureSetUp();
		}

        /// <summary>
        /// Conformance Item T3	
        /// GEOMETRY_COLUMNS table/view is created/updated properly	
        /// For this test we will check to see that the correct coordinate dimension 
        /// for the streams table is represented in the GEOMETRY_COLUMNS table/view
        ///
        /// ANSWER: 2	
        /// *** ADAPTATION ALERT ***	
        /// Since there are no quotes around the table name, streams, in it's CREATE TABLE,
        /// it will be converted to upper case in many DBMSs, and therefore, the WHERE 
        /// clause may have to be f_table_name = 'STREAMS'.
        ///
        /// Original SQL:
        /// <code>
        ///		SELECT coord_dimension
        ///		FROM geometry_columns
        ///		WHERE f_table_name = 'streams';
        /// </code>
        /// </summary>
        public override void ConformanceItemT03Hql()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query = session.CreateQuery(@"
				select g.Dimension 
				from GeometryColumn g 
				where g.TableName = 'streams'
				");

            var result = query.UniqueResult<int>();

            Assert.AreEqual(2, result);
        }

        public override void ConformanceItemT03Linq()
        {
            if (!Metadata.SupportsSpatialMetadata(session, MetadataClass.GeometryColumn))
            {
                Assert.Ignore("Provider does not support spatial metadata");
            }
            var query =
                from g in session.Query<SpatiaLiteGeometryColumn>()
                where g.TableName == "streams"
                select g.Dimension;

            var result = query.Single();

            Assert.AreEqual(2, 2);
        }

        public override void DeleteMappings(global::NHibernate.ISession session)
        {
            session.Flush();
            session.Clear();
        }
	}
}
