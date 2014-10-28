using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Driver;
using Environment = NHibernate.Cfg.Environment;
using Settings = Tests.NHibernate.Spatial.Properties.Settings;
using NHibernateFactory = NHibernate.Bytecode.DefaultProxyFactoryFactory;

namespace Tests.NHibernate.Spatial
{
	internal static class TestConfigurationOgcConformance
	{
		public static void Configure(Configuration configuration)
		{
			IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
			properties[Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName;
			properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
			properties[Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName;
			properties[Environment.ConnectionString] = Settings.Default.ConnectionString;
            //properties[Environment.]
            //properties[Environment.Hbm2ddlAuto] = "create-drop";
            //properties[Environment.PropertyBytecodeProvider]
			configuration.SetProperties(properties);
		}

	}
    internal static class TestConfigurationCriteria
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = Settings.Default.ConnectionStringCriteria;
            //properties[Environment.Hbm2ddlAuto] = "create-drop";
            configuration.SetProperties(properties);
        }

    }

    internal static class TestConfigurationProjection
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = Settings.Default.ConnectionStringProjection;
            //properties[Environment.Hbm2ddlAuto] = "create-drop";
            configuration.SetProperties(properties);
        }

    }
    
    internal static class TestConfigurationNts
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = Settings.Default.ConnectionStringNts;
            //properties[Environment.Hbm2ddlAuto] = "create-drop";
            configuration.SetProperties(properties);
        }

    }

    internal static class TestConfigurationSpatialQueries
    {
        public static void Configure(Configuration configuration)
        {
            IDictionary<string, string> properties = new Dictionary<string, string>();
            properties[Environment.ProxyFactoryFactoryClass] = typeof(NHibernateFactory).AssemblyQualifiedName;
            properties[Environment.Dialect] = typeof(SpatiaLiteDialect).AssemblyQualifiedName;
            properties[Environment.ConnectionProvider] = typeof(DebugConnectionProvider).AssemblyQualifiedName;
            properties[Environment.ConnectionDriver] = typeof(SpatiaLiteDriver).AssemblyQualifiedName;
            properties[Environment.ConnectionString] = Settings.Default.ConnectionStringSpatialQueries;
            //properties[Environment.Hbm2ddlAuto] = "create-drop";
            configuration.SetProperties(properties);
        }

    }

}
