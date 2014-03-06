using NHibernate.Cfg;
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

        [Ignore("SpatiaLite returns 'XY' instead of 2")]
        public override void ConformanceItemT03Hql()
        {
            //base.ConformanceItemT03Hql();
        }
        [Ignore("SpatiaLite returns 'XY' instead of 2")]
        public override void ConformanceItemT03Linq()
        {
            //base.ConformanceItemT03Linq();
        }

        public override void DeleteMappings(global::NHibernate.ISession session)
        {
            session.Flush();
            session.Clear();
        }
        //[Test]
        //public override void ConformanceItemT40Hql()
        //{
        //    SpatiaLite2TestsUtil.IgnoreIfAffectedByIssue22(_postGisVersion);
        //    base.ConformanceItemT40Hql();
        //}

        //[Test]
        //public override void ConformanceItemT40Linq()
        //{
        //    SpatiaLite2TestsUtil.IgnoreIfAffectedByIssue22(_postGisVersion);
        //    base.ConformanceItemT40Linq();
        //}

        //[Test]
        //public override void ConformanceItemT51Hql()
        //{
        //    SpatiaLite2TestsUtil.IgnoreIfAffectedByIssue22(_postGisVersion);
        //    base.ConformanceItemT51Hql();
        //}

        //[Test]
        //public override void ConformanceItemT51Linq()
        //{
        //    SpatiaLite2TestsUtil.IgnoreIfAffectedByIssue22(_postGisVersion);
        //    base.ConformanceItemT51Linq();
        //}
	}
}
