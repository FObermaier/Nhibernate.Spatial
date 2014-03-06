using System;
using System.Text;
using System.Threading;
using NHibernate.Dialect;
using NHibernate.Spatial.Dialect.Function;
using NHibernate.Spatial.Metadata;
using NHibernate.Spatial.Type;
using NHibernate.SqlCommand;
using NHibernate.Type;

namespace NHibernate.Spatial.Dialect
{
    public class SpatiaLiteDialect : SQLiteDialect, ISpatialDialect
    {
        private static readonly IType GeometryTypeInstance = new CustomType(typeof(SpatiaLiteGeometryType), null);

        private const string IntersectionAggregateName = "NHSP_IntersectionAggregate";

        public SpatiaLiteDialect()
        {
            SpatialDialect.LastInstantiated = this;
            RegisterBasicFunctions();
            RegisterFunctions();
        }

        #region Functions registration

        private void RegisterBasicFunctions()
        {
            // Relations
            RegisterSpatialFunction(SpatialRelation.Contains);
            RegisterSpatialFunction(SpatialRelation.CoveredBy);
            RegisterSpatialFunction(SpatialRelation.Covers);
            RegisterSpatialFunction(SpatialRelation.Crosses);
            RegisterSpatialFunction(SpatialRelation.Disjoint);
            RegisterSpatialFunction(SpatialRelation.Equals);
            RegisterSpatialFunction(SpatialRelation.Intersects);
            RegisterSpatialFunction(SpatialRelation.Overlaps);
            RegisterSpatialFunction(SpatialRelation.Touches);
            RegisterSpatialFunction(SpatialRelation.Within);

            // Analysis
            RegisterSpatialFunction(SpatialAnalysis.Buffer);
            RegisterSpatialFunction(SpatialAnalysis.ConvexHull);
            RegisterSpatialFunction(SpatialAnalysis.Difference);
            RegisterSpatialFunction(SpatialAnalysis.Distance);
            RegisterSpatialFunction(SpatialAnalysis.Intersection);
            RegisterSpatialFunction(SpatialAnalysis.SymDifference);
            RegisterSpatialFunction(SpatialAnalysis.Union);

            // Validations
            RegisterSpatialFunction(SpatialValidation.IsClosed);
            RegisterSpatialFunction(SpatialValidation.IsEmpty);
            RegisterSpatialFunction(SpatialValidation.IsRing);
            RegisterSpatialFunction(SpatialValidation.IsSimple);
            RegisterSpatialFunction(SpatialValidation.IsValid);
        }

        private void RegisterFunctions()
        {
            RegisterSpatialFunction("Boundary");
            RegisterSpatialFunction("Centroid");
            RegisterSpatialFunction("EndPoint");
            RegisterSpatialFunction("Envelope");
            RegisterSpatialFunction("ExteriorRing");
            RegisterSpatialFunction("GeometryN", 2);
            RegisterSpatialFunction("InteriorRingN", 2);
            RegisterSpatialFunction("PointN", 2);
            RegisterSpatialFunction("PointOnSurface");
            RegisterSpatialFunction("Simplify", 2);
            RegisterSpatialFunction("StartPoint");
            RegisterSpatialFunction("Transform", 2);

            RegisterSpatialFunction("GeomCollFromText", 2);
            RegisterSpatialFunction("GeomCollFromWKB", 2);
            RegisterSpatialFunction("GeomFromText", 2);
            RegisterSpatialFunction("GeomFromWKB", 2);
            RegisterSpatialFunction("LineFromText", 2);
            RegisterSpatialFunction("LineFromWKB", 2);
            RegisterSpatialFunction("PointFromText", 2);
            RegisterSpatialFunction("PointFromWKB", 2);
            RegisterSpatialFunction("PolyFromText", 2);
            RegisterSpatialFunction("PolyFromWKB", 2);
            RegisterSpatialFunction("MLineFromText", 2);
            RegisterSpatialFunction("MLineFromWKB", 2);
            RegisterSpatialFunction("MPointFromText", 2);
            RegisterSpatialFunction("MPointFromWKB", 2);
            RegisterSpatialFunction("MPolyFromText", 2);
            RegisterSpatialFunction("MPolyFromWKB", 2);

            RegisterSpatialFunction("AsBinary", NHibernateUtil.Binary);

            RegisterSpatialFunction("AsText", NHibernateUtil.String);
            RegisterSpatialFunction("AsGML", NHibernateUtil.String);
            RegisterSpatialFunction("GeometryType", NHibernateUtil.String);

            RegisterSpatialFunction("Area", NHibernateUtil.Double);
            RegisterSpatialFunction("Length", "GLength",  NHibernateUtil.Double);
            RegisterSpatialFunction("X", NHibernateUtil.Double);
            RegisterSpatialFunction("Y", NHibernateUtil.Double);

            RegisterSpatialFunction("SRID", NHibernateUtil.Int32);
            RegisterSpatialFunction("Dimension", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumGeometries", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumInteriorRings", "NumInteriorRing", NHibernateUtil.Int32);
            RegisterSpatialFunction("NumPoints", NHibernateUtil.Int32);

            RegisterSpatialFunction("Relate", NHibernateUtil.Boolean, 3);
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType, int allowedArgsCount)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + standardName, new SpatialStandardSafeFunction(dialectName, returnedType, allowedArgsCount));
        }

        private void RegisterSpatialFunction(string standardName, string dialectName, IType returnedType)
        {
            RegisterSpatialFunction(standardName, dialectName, returnedType, 1);
        }

        private void RegisterSpatialFunction(string name, IType returnedType, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, name, returnedType, allowedArgsCount);
        }

        private void RegisterSpatialFunction(string name, IType returnedType)
        {
            RegisterSpatialFunction(name, name, returnedType);
        }

        private void RegisterSpatialFunction(string name, int allowedArgsCount)
        {
            RegisterSpatialFunction(name, GeometryType, allowedArgsCount);
        }

        private void RegisterSpatialFunction(string name)
        {
            RegisterSpatialFunction(name, GeometryType);
        }

        private void RegisterSpatialFunction(SpatialRelation relation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + relation, new SpatialRelationFunction(this, relation));
        }

        private void RegisterSpatialFunction(SpatialValidation validation)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + validation, new SpatialValidationFunction(this, validation));
        }

        private void RegisterSpatialFunction(SpatialAnalysis analysis)
        {
            RegisterFunction(SpatialDialect.HqlPrefix + analysis, new SpatialAnalysisFunction(this, analysis));
        }

        #endregion

        public IType GeometryType
        {
            get { return GeometryTypeInstance; }
        }

        public IGeometryUserType CreateGeometryUserType()
        {
            return new SpatiaLiteGeometryType();
        }

        public SqlString GetSpatialTransformString(object geometry, int srid)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add("Transform(")
                .AddObject(geometry)
                .Add(",")
                .Add(srid.ToString())
                .Add(")")
                .ToSqlString();
        }

        public SqlString GetSpatialAggregateString(object geometry, SpatialAggregate aggregate)
        {
			// PostGIS aggregate functions do not need prefix
			string aggregateFunction;
			switch (aggregate)
			{
				case SpatialAggregate.Collect:
					aggregateFunction = "Collect";
					break;
				case SpatialAggregate.Envelope:
			        throw new NotSupportedException("SpatialAggregate Extent/Envelope");
					aggregateFunction = "Envelope";
					break;
				case SpatialAggregate.Intersection:
			        throw new NotSupportedException("SpatialAggregate Intersection");
					aggregateFunction = "Intersection";
					break;
				case SpatialAggregate.Union:
					aggregateFunction = "GUnion";
					break;
				default:
					throw new ArgumentException("Invalid spatial aggregate argument");
			}
			return new SqlStringBuilder()
				.Add(aggregateFunction)
				.Add("(")
				.AddObject(geometry)
				.Add(")")
				.ToSqlString();
        }

        public SqlString GetSpatialAnalysisString(object geometry, SpatialAnalysis analysis, object extraArgument)
        {
            switch (analysis)
            {
                case SpatialAnalysis.Buffer:
                    if (!(extraArgument is Parameter || new SqlString(Parameter.Placeholder).Equals(extraArgument)))
                    {
                        extraArgument = Convert.ToString(extraArgument, System.Globalization.NumberFormatInfo.InvariantInfo);
                    }
                    return new SqlStringBuilder(6)
                        .Add(SpatialDialect.IsoPrefix)
                        .Add("Buffer(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.ConvexHull:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add("ConvexHull(")
                        .AddObject(geometry)
                        .Add(")")
                        .ToSqlString();
                case SpatialAnalysis.Difference:
                case SpatialAnalysis.Distance:
                case SpatialAnalysis.Intersection:
                case SpatialAnalysis.SymDifference:
                case SpatialAnalysis.Union:
                    return new SqlStringBuilder()
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(analysis.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(",")
                        .AddObject(extraArgument)
                        .Add(")")
                        .ToSqlString();
                default:
                    throw new ArgumentException("Invalid spatial analysis argument");
            }
        }

        public SqlString GetSpatialValidationString(object geometry, SpatialValidation validation, bool criterion)
        {
            return new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add(validation.ToString())
                .Add("(")
                .AddObject(geometry)
                .Add(")")
                .ToSqlString();
        }

        public SqlString GetSpatialRelateString(object geometry, object anotherGeometry, object pattern, bool isStringPattern, bool criterion)
        {
            if (pattern == null)
                throw new NotSupportedException("Computing Relate pattern string");
            
            var relateString = new SqlStringBuilder()
                .Add(SpatialDialect.IsoPrefix)
                .Add("Relate(")
                .AddObject(geometry)
                .Add(",")
                .AddObject(anotherGeometry)
                .Add(",");

            if (isStringPattern)
                relateString.Add("'");

            relateString.Add(pattern.ToString());

            if (isStringPattern)
                relateString.Add("'");

            relateString.Add(")");
            
            return relateString.ToSqlString();
        }

        public SqlString GetSpatialRelationString(object geometry, SpatialRelation relation, object anotherGeometry, bool criterion)
        {
            switch (relation)
            {
                case SpatialRelation.Covers:
                    var patterns = new[] {
						"T*****FF*",
						"*T****FF*",
						"***T**FF*",
						"****T*FF*",
					};
                    var builder = new SqlStringBuilder();
                    builder.Add("(");
                    for (var i = 0; i < patterns.Length; i++)
                    {
                        if (i > 0)
                            builder.Add(" OR ");
                        builder
                            .Add(SpatialDialect.IsoPrefix)
                            .Add("Relate")
                            .Add("(")
                            .AddObject(geometry)
                            .Add(", ")
                            .AddObject(anotherGeometry)
                            .Add(", '")
                            .Add(patterns[i])
                            .Add("')")
                            .ToSqlString();
                    }
                    builder.Add(")");
                    return builder.ToSqlString();
                case SpatialRelation.CoveredBy:
                    return GetSpatialRelationString(anotherGeometry, SpatialRelation.Covers, geometry, criterion);
                default:
                    return new SqlStringBuilder(6)
                        .Add(SpatialDialect.IsoPrefix)
                        .Add(relation.ToString())
                        .Add("(")
                        .AddObject(geometry)
                        .Add(", ")
                        .AddObject(anotherGeometry)
                        .Add(")")
                        .ToSqlString();
            }
        }

        public SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName,
            string tableName, Parameter parameter)
        {
            return new SqlStringBuilder(30)
#if useMBR                
                .Add("(MbrIntersects(")
#else
                .Add("(ST_Intersects(")
#endif
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(", ")
                .Add(parameter)
                .Add("))")
                .ToSqlString();
        }

        public SqlString GetSpatialFilterString(string tableAlias, string geometryColumnName, string primaryKeyColumnName, string tableName)
        {
            return new SqlStringBuilder(30)
#if useMBR                
                .Add("(MbrIntersects(")
#else
                .Add("(ST_Intersects(")
#endif
                .Add(tableAlias)
                .Add(".")
                .Add(geometryColumnName)
                .Add(", ")
                .AddParameter()
                .Add("))")
                .ToSqlString();
        }

        /// <summary>
        /// Gets the spatial create string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialCreateString(string schema)
        {
            //return "SELECT InitSpatialMetaData() WHERE CheckSpatialMetaData()=0;";
            return string.Empty;
        }

        public string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype)
        {
            return GetSpatialCreateString(schema, table, column, srid, subtype, 2);
        }

        public string GetSpatialCreateString(string schema, string table, string column, int srid, string subtype, int dimension)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT RecoverGeometryColumn('{0}','{1}',{2},'{3}', '{4}')",
                table, column, srid, subtype, ToXyzm(dimension));

            builder.Append(MultipleQueriesSeparator);
            
            builder.AppendFormat("UPDATE \"geometry_columns\" SET \"type\"='{2}' WHERE \"f_table_name\"='{0}' AND \"f_geometry_column\"='{1}'",
                table, column, subtype);

            builder.Append(MultipleQueriesSeparator);

            builder.AppendFormat("SELECT CreateSpatialIndex('{0}','{1}')",
                table, column);

            builder.Append(MultipleQueriesSeparator);

            return builder.ToString();
        }

        private static string ToXyzm(int dimension)
        {
            switch (dimension)
            {
                case 3:
                    return "XYZ";
                case 4:
                    return "XYZM";
            }
            return "XY";
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema)
        {
            return String.Empty;
        }

        /// <summary>
        /// Gets the spatial drop string.
        /// </summary>
        /// <param name="schema">The schema.</param>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public string GetSpatialDropString(string schema, string table, string column)
        {
            var builder = new StringBuilder();

            builder.AppendFormat("SELECT DisableSpatialIndex('{0}','{1}')",
                table, column);

            builder.Append(MultipleQueriesSeparator);

            builder.AppendFormat("SELECT DiscardGeometryColumn('{0}','{1}')",
                table, column);

            builder.Append(MultipleQueriesSeparator);

            builder.AppendFormat("DELETE FROM geometry_table where f_table_name = '{0}' AND f_geometry_column = '{1}';",
                table, column);
            /*
            builder.Append(MultipleQueriesSeparator);
            builder.AppendFormat("ALTER TABLE {0} DROP COLUMN {1}"
                , QuoteForTableName(table)
                , QuoteForColumnName(column)
                );
            
            builder.Append(MultipleQueriesSeparator);
            */
            return builder.ToString();
        }

        public bool SupportsSpatialMetadata(MetadataClass metadataClass)
        {
            return true;
        }

        public string MultipleQueriesSeparator
        {
            get { return ";"; }
        }
    }
}