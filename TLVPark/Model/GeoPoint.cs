using System;
using System.Runtime.Serialization;

namespace TLVPark.Model
{
    /// <summary>
    /// Represents a geographical point with a longtitude and a latitude
    /// </summary>
    [DataContract]
    public class GeoPoint : ICloneable
    {
        #region Properties

        /// <summary>
        /// Represents the longtitude of the point
        /// </summary>
        [DataMember]
        public virtual double Longtitude { get; set; }
        /// <summary>
        /// Represents the latitude of the point
        /// </summary>
        [DataMember]
        public virtual double Latitude { get; set; }

        #endregion

        #region Constructors

        public GeoPoint(double longtitude, double latitude)
        {
            Longtitude = longtitude;
            Latitude = latitude;
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(GeoPoint point)
        {
            return Math.Sqrt(Math.Pow((Longtitude - point.Longtitude), 2) + Math.Pow((Latitude - point.Latitude), 2));
        }

        #endregion

        #region ICloneable Methods

        public object Clone()
        {
            return new GeoPoint(Longtitude, Latitude);
        }

        #endregion

    }
}
