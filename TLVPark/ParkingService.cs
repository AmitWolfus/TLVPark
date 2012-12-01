using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Configuration;
using TLVPark.DataAccess;
using TLVPark.Model;

namespace TLVPark
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ParkingService : IParkingService
    {
        #region IParkingService Methods

        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "Parkings?latitude={latitude}&longtitude={longtitude}&radius={radius}")]
        public List<Parking> GetParkingsByGeo(string latitude, string longtitude, string radius)
        {
            // Convert the request parameters into the right type
            double longt;
            double lat;
            double rad;
            if (!double.TryParse(longtitude, out longt)
                || !double.TryParse(latitude, out lat)
                || !double.TryParse(radius, out rad))
            {
                // The parameters aren't of the right type, close the request
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            var point = new GeoPoint(longt, lat);
            try
            {
                List<Parking> parkings;
                using (var dataAccess = new ParkingDataAccess())
                {
                    parkings = dataAccess.GetParkingsByGeo(point, rad).ToList();
                }
                return parkings;
            }
            catch (Exception ex)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.PaymentRequired;
                WebOperationContext.Current.OutgoingResponse.StatusDescription = "Failed";
                return new List<Parking>(){new Parking(){Name = ex.GetType().Name, StreetName = ex.Message}};
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Xml, UriTemplate = "Business?businessId={businessId}&businessType={businessType}")]
        public List<Parking> GetParkingsByBusiness(string businessId, string businessType)
        {
            int id;
            if (!int.TryParse(businessId, out id))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
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
