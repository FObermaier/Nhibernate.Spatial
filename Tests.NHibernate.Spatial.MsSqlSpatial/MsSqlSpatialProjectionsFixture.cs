using System;
using NHibernate.Cfg;
using NUnit.Framework;

namespace Tests.NHibernate.Spatial
{
	[TestFixture]
	public class MsSqlSpatialProjectionsFixture : ProjectionsFixture
	{
		protected override void Configure(Configuration configuration)
		{
			TestConfiguration.Configure(configuration);
		}

	    protected override Type GeometryType
	    {
	        get { return typeof (global::NHibernate.Spatial.Type.MsSqlSpatialGeometryType); }
	    }
	}
}
