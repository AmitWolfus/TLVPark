using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using TLVPark.DataAccess;
using TLVPark.Model;

namespace TLVPark
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ParkingService : IParkingService
    {
        #region IParkingService Methods

        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "ParkingsByGeo({latitude},{longtitude},{radius})")]
        public List<Parking> GetParkingsByGeo(string latitude, string longtitude, string radius)
        {
            var longt = double.Parse(longtitude);
            var lat = double.Parse(latitude);
            var rad = double.Parse(radius);
            var point = new GeoPoint(longt, lat);
            List<Parking> parkings; 
            using (var dataAccess = new ParkingDataAccess())
            {
                parkings = dataAccess.GetParkingsByGeo(point, rad).ToList();
            }
            return parkings;
        }

        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "ParkingsByBusiness({businessId},{businessType}")]
        public List<Parking> GetParkingsByBusiness(string businessId, string businessType)
        {
            var id = int.Parse(businessId);
            List<Parking> parkings;
            using (var dataAccess = new ParkingDataAccess())
            {
                parkings = dataAccess.GetParkingByBusiness(id).ToList();
            }
            // TODO: Add more logic
            return parkings;
        }

        #endregion
    }
}
