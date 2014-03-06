using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class SpatiaLiteCriteriaFixture : CriteriaFixture
	{
	    public SpatiaLiteCriteriaFixture()
	    {
            if (System.IO.File.Exists("CriteriaTest.sqlite"))
                System.IO.File.Delete("CriteriaTest.sqlite");
	    }
        protected override void Configure(Configuration configuration)
		{
			TestConfigurationCriteria.Configure(configuration);
		}

        [Ignore("SpatiaLite ST_EMPTY() does not differentiate between null and spatial empty.")]
        public override void CountSpatialEmpty()
        {
            base.CountSpatialEmpty();
        }
	}
}
