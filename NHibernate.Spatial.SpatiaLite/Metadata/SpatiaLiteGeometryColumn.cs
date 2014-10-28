using System;
using NHibernate.Hql.Ast.ANTLR;

namespace NHibernate.Spatial.Metadata
{
    public class SpatiaLiteGeometryColumn : GeometryColumn
    {
        /// <summary>
        /// Gets or sets the geometry dimension.
        /// </summary>
        /// <value>The dimension.</value>
        public override int Dimension 
        {
            get
            {
                switch (CoordDimension)
                {
                    case "XY":
                        return 2;
                    case "XYM":
                    case "XYZ":
                        return 3;
                    case "XYZM":
                        return 4;
                }
                return 2;
            }
            set
            {
                switch (value)
                {
                    //case 2:
                    default:
                        CoordDimension = "XY";
                        break;
                    case 3:
                        //TODO AddDefault for 3
                        CoordDimension = "XYZ";
                        break;
                    case 4:
                        CoordDimension = "XYZM";
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the coordinate dimension
        /// </summary>
        public virtual string CoordDimension { get; set; }

        /// <summary>
        /// Gets or sets the geometry subtype.
        /// </summary>
        /// <value>The subtype.</value>
        public override string Subtype
        {
            get
            {
                switch (GeometryType)
                {
                    case 1:
                        return "POINT";
                    case 2:
                        return "LINESTRING";
                    case 3:
                        return "POLYGON";
                    case 4:
                        return "MULTIPOINT";
                    case 5:
                        return "MULTILINESTRING";
                    case 6:
                        return "MULTIPOLYGON";
                    case 7:
                        return "GEOMETRYCOLLECTION";
                }
                throw new Exception("Should never reach here");
            }
            set
            {
                switch (value)
                {
                    case "POINT":
                        GeometryType = 1;
                        break;
                    case "LINESTRING":
                        GeometryType = 2;
                        break;
                    case "POLYGON":
                        GeometryType = 3;
                        break;
                    case "MULTIPOINT":
                        GeometryType = 4;
                        break;
                    case "MULTILINESTRING":
                        GeometryType = 5;
                        break;
                    case "MULTIPOLYGON":
                        GeometryType = 6;
                        break;
                    case "GEOMETRYCOLLECTION":
                        GeometryType = 7;
                        break;
                    default:
                        throw new Exception("Should never reach here");
                }
            }
        }

        /// <summary>
        /// Gets or sets the integer value defining the <see cref="Subtype"/>.
        /// </summary>
        public virtual int GeometryType 
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating which (if any) spatial index is enabled
        /// </summary>
        public virtual int SpatialIndex { get; set; }
    }
}