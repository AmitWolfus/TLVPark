using System;
using System.Collections.Generic;
using TLVPark.Model;

namespace TLVPark.DataAccess
{
    public interface IParkingDataAccess : IDisposable
    {
        /// <summary>
        /// Gets all parkings in a radius from a given point.
        /// </summary>
        /// <param name="point">The center of the circle</param>
        /// <param name="radius">The radius to look within</param>
        /// <returns>The parkings within the given area</returns>
        IEnumerable<Parking> GetParkingsByGeo(GeoPoint point, double radius);
        /// <summary>
        /// Gets the recomended parkings for a business
        /// </summary>
        /// <param name="businessId">The id of the business</param>
        /// <returns>The parkings for the given business</returns>
        IEnumerable<Parking> GetParkingByBusiness(int businessId);
    }
}
