using System;

namespace NHibernate.Spatial.Linq
{
	public class SpatialLinqMethodException : Exception
	{
		public SpatialLinqMethodException()
			: this("Method to use only in Linq expressions")
		{
		}

	    public SpatialLinqMethodException(string message)
            :base(message)
	    {
	    }
	}
}