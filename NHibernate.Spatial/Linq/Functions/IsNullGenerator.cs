using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using GeoAPI.Geometries;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Type;
using NHibernate.UserTypes;

namespace NHibernate.Spatial.Linq.Functions
{
	public class IsNullGenerator : BaseHqlGeneratorForMethod
	{
		public IsNullGenerator()
		{
			SupportedMethods = new []
			{
				ReflectionHelper.GetMethodDefinition<IGeometry>(g => g.IsNull())
			};
		}

		public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
		{
			var isNull = treeBuilder.IsNull(visitor.Visit(arguments[0]).AsExpression());
			return isNull;
		}
	}

    public class ToDbGeometryGenerator : BaseHqlGeneratorForMethod
    {
        public ToDbGeometryGenerator()
        {
            SupportedMethods = new[]
            {
                ReflectionHelper.GetMethodDefinition<IGeometry>(g => g.ToDbGeometry())
            };
        }

        private static object ToUserType(IGeometry geometry)
        {
            return SpatialDialect.LastInstantiated.CreateGeometryUserType().ToNative(geometry);
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var ce = arguments[0] as ConstantExpression;
            if (ce == null)
                throw new ArgumentException();
            return BuildHql(method, targetObject,
                new ReadOnlyCollection<Expression>(new[] {Expression.Constant(ToUserType((IGeometry) ce.Value))}),
                treeBuilder, visitor);
        }
    }
}