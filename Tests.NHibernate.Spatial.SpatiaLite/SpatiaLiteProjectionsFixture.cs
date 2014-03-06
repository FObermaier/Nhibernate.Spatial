using System;
using System.IO;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Criterion;
using NHibernate.Spatial.Dialect;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class SpatiaLiteProjectionsFixture : ProjectionsFixture
	{
	    public SpatiaLiteProjectionsFixture()
	    {
	        if (File.Exists("ProjectionTest.sqlite"))
                File.Delete("ProjectionTest.sqlite");
	    }
        
        protected override void Configure(Configuration configuration)
		{
			TestConfigurationProjection.Configure(configuration);
		}

        protected override Type GeometryType
        {
            get { return typeof(global::NHibernate.Spatial.Type.SpatiaLiteGeometryType); }
        }

        /*
        [Ignore("Collect not implemented in SpatiaLite")]
        public override void CollectAll()
        {
            base.CollectAll();
        }
         */

        [Ignore("Envelope aggregate not implemented in SpatiaLite")]
        public override void EnvelopeAll()
        {
            base.EnvelopeAll();
        }

        [Ignore("Intersection aggregate not implemented in SpatiaLite")]
        public override void IntersectionAll()
        {
            base.IntersectionAll();
        }
	}

}
