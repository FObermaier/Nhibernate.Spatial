using System.Xml;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NHibernate.Spatial.Dialect;
using NHibernate.Spatial.Linq.Functions;

namespace NHibernate.Spatial.Linq
{
	public static class SpatialLinqExtensions
	{

        /// <summary>
        /// A fully compatible null checking. Use instead of " == null " expression.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using an equality to null lambda expression throws an exception in SQL Server
        /// ("Invalid operator for data type. Operator equals equal to, type equals geometry.")
        /// because NHibernate is generating an HQL expression like this:
        /// </para>
        /// <code>
        ///     (t.geom is null) and (null is null) or t.geom = null
        /// </code>
        /// <para>
        /// Using this extension method, we generate just the following HQL:
        /// </para>
        /// <code>
        ///     t.geom is null
        /// </code>
        /// </remarks>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static bool IsNull(this IGeometry geometry)
        { return geometry == null; }

		public static int GetDimension(this IGeometry geometry)
		{ return (int)geometry.Dimension; }	
		
		public static IGeometry Simplify(this IGeometry geometry, double distance)
		{
		    return geometry.Simplify(distance);
		}	

		public static IGeometry Transform(this IGeometry geometry, int srid)
		{ throw new SpatialLinqMethodException(); }

        public static IGeometryCollection ToGeometryCollection(this string text, int srid)
        {
            var res = ToGeometry(text, srid);
            if (res==null)
                return new GeometryCollection(null) { SRID = srid };

            if (res.OgcGeometryType == OgcGeometryType.GeometryCollection) 
                return (IGeometryCollection)res;

	        return res.Factory.CreateGeometryCollection(new []{res});
	    }

	    public static IGeometryCollection ToGeometryCollection(this byte[] wkb, int srid)
	    {
	        var res = ToGeometry(wkb, srid);
            if (res == null)
                return new GeometryCollection(null) { SRID = srid };

            if (res.OgcGeometryType == OgcGeometryType.GeometryCollection)
                return (IGeometryCollection)res;

            return res.Factory.CreateGeometryCollection(new[] { res });
	    }

	    public static IGeometry ToGeometry(this string text, int srid)
	    {
	        var factory = GeoAPI.GeometryServiceProvider.Instance.CreateGeometryFactory(srid);
	        if (string.IsNullOrEmpty(text))
	            factory.CreateGeometryCollection(null);

	        var reader = new WKTReader(factory);
	        var res = reader.Read(text);
	        res.SRID = srid;
	        return res;
	    }

	    public static IGeometry ToGeometry(this byte[] wkb, int srid)
	    {
	        if (wkb == null || wkb.Length == 0)
	        {
	            return GeoAPI.GeometryServiceProvider.Instance.CreateGeometryFactory(srid).CreateGeometryCollection(null);
	        }

	        var reader = new WKBReader(GeoAPI.GeometryServiceProvider.Instance);
	        var res = reader.Read(wkb);
	        res.SRID = srid;

	        return res;
	    }

	    public static ILineString ToLineString(this string text, int srid)
	    {
	        var res = text.ToGeometry(srid);
	        if (res.OgcGeometryType == OgcGeometryType.LineString)
	            return (ILineString)res;

            throw new SpatialLinqMethodException(string.Format("Failed to build LineString from\n{0}", text));
	    }

	    public static ILineString ToLineString(this byte[] wkb, int srid)
	    {
            var res = wkb.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.LineString)
                return (ILineString)res;

            throw new SpatialLinqMethodException(string.Format("Failed to build LineString from\n{0}", WKBWriter.ToHex(wkb)));
        }

	    public static IPoint ToPoint(this string text, int srid)
	    {
            var res = text.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.Point)
                return (IPoint)res;

            throw new SpatialLinqMethodException(string.Format("Failed to build Point from\n{0}", text));
        }

	    public static IPoint ToPoint(this byte[] wkb, int srid)
	    {
            var res = wkb.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.Point)
                return (IPoint)res;

            throw new SpatialLinqMethodException(string.Format("Failed to build Point from\n{0}", WKBWriter.ToHex(wkb)));
        }	

		public static IPolygon ToPolygon(this string text, int srid)
        {
            var res = text.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.Polygon)
                return (IPolygon)res;

		    if (res.OgcGeometryType == OgcGeometryType.LineString)
		    {
		        var lineString = (ILineString) res;
                if (lineString.IsClosed)
                    return res.Factory.CreatePolygon(lineString.CoordinateSequence);
		    }

            throw new SpatialLinqMethodException(string.Format("Failed to build Polygon from\n{0}", text));
        }

		public static IPolygon ToPolygon(this byte[] wkb, int srid)
        {
            var res = wkb.ToGeometry(srid);

            if (res.OgcGeometryType == OgcGeometryType.Polygon)
                return (IPolygon)res;

            if (res.OgcGeometryType == OgcGeometryType.LineString)
            {
                var lineString = (ILineString)res;
                if (lineString.IsClosed)
                    return res.Factory.CreatePolygon(lineString.CoordinateSequence);
            }

            throw new SpatialLinqMethodException(string.Format("Failed to build Polygon from\n{0}", WKBWriter.ToHex(wkb)));
        }	

		public static IMultiLineString ToMultiLineString(this string text, int srid)
        {
            var res = text.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiLineString)
                return (IMultiLineString)res;

            if (res.OgcGeometryType == OgcGeometryType.LineString)
                return res.Factory.CreateMultiLineString(new[] { (ILineString)res });

            throw new SpatialLinqMethodException(string.Format("Failed to build MultiLineString from\n{0}", text));
        }

		public static IMultiLineString ToMultiLineString(this byte[] wkb, int srid)
        {
            var res = wkb.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiLineString)
                return (IMultiLineString)res;

            if (res.OgcGeometryType == OgcGeometryType.LineString)
                return res.Factory.CreateMultiLineString(new [] {(ILineString)res});

            throw new SpatialLinqMethodException(string.Format("Failed to build MultiLineString from\n{0}", WKBWriter.ToHex(wkb)));
        }	

		public static IMultiPoint ToMultiPoint(this string text, int srid)
        {
            var res = text.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiPoint)
                return (IMultiPoint)res;

            if (res.OgcGeometryType == OgcGeometryType.Point)
                return res.Factory.CreateMultiPoint(new[] { (IPoint)res });

            throw new SpatialLinqMethodException(string.Format("Failed to build MultiPoint from\n{0}", text));
        }

		public static IMultiPoint ToMultiPoint(this byte[] wkb, int srid)
        {
            var res = wkb.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiPoint)
                return (IMultiPoint)res;

            if (res.OgcGeometryType == OgcGeometryType.Point)
                return res.Factory.CreateMultiPoint(new[] { (IPoint)res });

            throw new SpatialLinqMethodException(string.Format("Failed to build MultiPoint from\n{0}", WKBWriter.ToHex(wkb)));
        }	

		public static IMultiPolygon ToMultiPolygon(this string text, int srid)
        {
            var res = text.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiPolygon)
                return (IMultiPolygon)res;

		    if (res.OgcGeometryType == OgcGeometryType.Polygon)
		        return res.Factory.CreateMultiPolygon(new[] {(IPolygon) res});

            throw new SpatialLinqMethodException(string.Format("Failed to build MultiPolygon from\n{0}", text));
        }

		public static IMultiPolygon ToMultiPolygon(this byte[] wkb, int srid)
        {
            var res = wkb.ToGeometry(srid);
            if (res.OgcGeometryType == OgcGeometryType.MultiPolygon)
                return (IMultiPolygon)res;

            if (res.OgcGeometryType == OgcGeometryType.Polygon)
                return res.Factory.CreateMultiPolygon(new[] { (IPolygon)res });


            throw new SpatialLinqMethodException(string.Format("Failed to build MultiPolygon from\n{0}", WKBWriter.ToHex(wkb)));
        }	
	
	}
}