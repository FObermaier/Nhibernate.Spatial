using System;
using System.IO;
using NHibernate.Cfg;
using NUnit.Framework;
using Tests.NHibernate.Spatial.NtsTestCases;

namespace Tests.NHibernate.Spatial
{
    [TestFixture]
    public class SpatiaLiteNtsTestCasesFixture : NtsTestCasesFixture
    {
        protected override void Configure(Configuration configuration)
        {
            TestConfigurationNts.Configure(configuration);
        }

        protected override void OnTestFixtureSetUp()
        {
            SpatiaLiteTestsUtil.GetSpatialLiteVersion(this.sessions);
            base.OnTestFixtureSetUp();
        }

        [Ignore("Relate function returning the DE9M string not supported.")]
        public override void StringRelate()
        {
            base.StringRelate();
        }
    }
}
