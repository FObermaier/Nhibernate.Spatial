using System;
using GeoAPI.Geometries;
using GeoAPI.IO;
using NetTopologySuite.IO;

namespace NHibernate.Spatial.Type 
{
    public class SpatiaLiteGeometryType : GeometryTypeBase<byte[]>
    {
        /// <summary>
        /// Value indicating whether the multi coordinate geometries should be stored in a packed way.
        /// That is initial coordinate with its double ordinate values, for subsequent coordinates only store
        /// the offset.
        /// </summary>
        private bool _compressed;
        
        /// <summary>
        /// Force coordinates to have a Z-ordinate value
        /// </summary>
        private bool _forceZ;

        /// <summary>
        /// Force coordinates to have a M ordinate value
        /// </summary>
        private bool _forceM;

        private IBinaryGeometryReader _reader;
        private IBinaryGeometryWriter _writer;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SpatiaLiteGeometryType"/> class.
        /// </summary>
        public SpatiaLiteGeometryType() : base(NHibernateUtil.BinaryBlob)
        {
            _reader = new GaiaGeoReader();
            _writer = new GaiaGeoWriter();
        }

        protected override byte[] FromGeometry(object value)
        {
            var geom = value as IGeometry;
            if (geom == null)
                return null;
            SetDefaultSRID(geom);
            
            return _writer.Write(geom);
        }

        protected override IGeometry ToGeometry(object value)
        {
            var blob = value as byte[];
            if (blob == null)
                return null;

            IGeometry geom = _reader.Read(blob);
			SetDefaultSRID(geom);
            return geom;
        }

        public override void SetParameterValues(System.Collections.Generic.IDictionary<string, string> parameters)
        {
            if (parameters == null)
                return;
            
            base.SetParameterValues(parameters);

            string parameter;
            if (parameters.TryGetValue("compressed", out parameter))
            {
                if (!String.IsNullOrEmpty(parameter))
                {
                    Boolean.TryParse(parameter, out _compressed);
                }
            }
            if (parameters.TryGetValue("forcez", out parameter))
            {
                if (!String.IsNullOrEmpty(parameter))
                {
                    Boolean.TryParse(parameter, out _forceZ);
                }
            }
            if (parameters.TryGetValue("forcem", out parameter))
            {
                if (!String.IsNullOrEmpty(parameter))
                {
                    Boolean.TryParse(parameter, out _forceM);
                }
            }

            var ordinates = Ordinates.XY;
            if (_forceZ)
            {
                ordinates |= Ordinates.Z;
                //ToDo check and/or update dimension
            }
            if (_forceM)
            {
                ordinates |= Ordinates.M;
                //ToDo check and/or update dimension
            }

            _reader = new GaiaGeoReader {HandleOrdinates = ordinates};
            _writer = new GaiaGeoWriter {HandleOrdinates = ordinates, UseCompressed = _compressed};
        }

#if DEBUG
        public IGeometry TestToGeometry(object value)
        {
            return ToGeometry(value);
        }

        public byte[] TestFromGeometry(object value)
        {
            return FromGeometry(value);
        }
#endif

    }
}
