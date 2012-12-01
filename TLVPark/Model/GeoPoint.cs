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
            return Distance(this, point);
            return Math.Sqrt(Math.Pow((Longtitude - point.Longtitude), 2) + Math.Pow((Latitude - point.Latitude), 2));
        }

        public static double Distance(GeoPoint point1, GeoPoint point2)
        {
            var lat1 = point1.Latitude;
            var lat2 = point2.Latitude;
            var lon1 = point1.Longtitude;
            var lon2 = point2.Longtitude;
            var R = 6371000000; // m (change this constant to get miles)
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;
            // The current result is in mm, division is done to return meters
            return Math.Round(d) / 1000;
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
