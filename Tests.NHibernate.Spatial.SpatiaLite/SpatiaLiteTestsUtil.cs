using NHibernate;

namespace Tests.NHibernate.Spatial
{
	public class SpatiaLiteTestsUtil
	{

		public static string GetSpatialLiteVersion(ISessionFactory sessionFactory)
		{
			using (ISession session = sessionFactory.OpenSession())
			{
				return (string)session
                    .CreateSQLQuery("SELECT spatialite_version();")
					.UniqueResult();
			}
		}
	}
}