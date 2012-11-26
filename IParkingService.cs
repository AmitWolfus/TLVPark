using System.Collections.Generic;
using System.ServiceModel;
using TLVPark.Model;

namespace TLVPark
{
    [ServiceContract]
    public interface IParkingService
    {
        /// <summary>
        /// Returns the available parkings in a given circle
        /// </summary>
        /// <param name="latitude">The latitude of the circle's center point</param>
        /// <param name="longtitude">The longtitude of the circle's center</param>
        /// <param name="radius">The radius of the area</param>
        /// <returns>A list of all the available parkings</returns>
        [OperationContract]
        List<Parking> GetParkingsByGeo(string latitude, string longtitude, string radius);
        /// <summary>
        /// Returns the recommended parkings for the desired business
        /// </summary>
        /// <param name="businessId">The Identifier of the business</param>
        /// <param name="businessType">The type of the business</param>
        /// <returns>The availavle parkings</returns>
        [OperationContract]
        List<Parking> GetParkingsByBusiness(string businessId, string businessType);
    }
}
