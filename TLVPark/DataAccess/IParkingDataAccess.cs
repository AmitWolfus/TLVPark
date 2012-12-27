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
        /// Gets up to the amount of desired parkings in a radius from a given point,
        /// the parkings will be taken starting from the closest to the center and going
        /// out from there.
        /// </summary>
        /// <param name="point">The center of the circle</param>
        /// <param name="radius">The radius of the area</param>
        /// <param name="parkingsToTake">The maximum amount of parkings to return</param>
        /// <returns>Up to the desired amount of parkings in the area</returns>
        IEnumerable<Parking> GetParkingsByGeo(GeoPoint point, double radius, int parkingsToTake);
        /// <summary>
        /// Gets the recomended parkings for a business
        /// </summary>
        /// <param name="businessId">The id of the business</param>
        /// <returns>The parkings for the given business</returns>
        IEnumerable<Parking> GetParkingByBusiness(long businessId);
        /// <summary>
        /// Reports the current state for a parking
        /// </summary>
        /// <param name="id">The parking's id</param>
        /// <param name="state">The current state of the parking</param>
        void SetStatusForParking(int id, ParkingState state);
    }
}
