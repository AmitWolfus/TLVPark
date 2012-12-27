using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web.Configuration;
using TLVPark.DataAccess;
using TLVPark.DataAccess.FacebookQuerying;
using TLVPark.Model;

namespace TLVPark
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ParkingService : IParkingService
    {
        #region IParkingService Methods

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Parkings?latitude={latitude}&longtitude={longtitude}&radius={radius}")]
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
                
                string message = string.Empty;
                while (ex != null)
                {
                    message = string.Format("{0}\nType:{1}\nMessage:{2}", message, ex.GetType().Name, ex.Message);
                    ex = ex.InnerException;
                }
                return new List<Parking>() { new Parking() { Name = message } };
            }
        }

        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "Business?businessId={businessId}&businessType={businessType}")]
        public List<Parking> GetParkingsByBusiness(string businessId, string businessType)
        {
            long id;
            if (!long.TryParse(businessId, out id))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
                return null;
            }
            List<Parking> parkings;
            using (var dataAccess = new ParkingDataAccess())
            {
                parkings = dataAccess.GetParkingByBusiness(id).ToList();
            }
            if (!parkings.Any())
            {
                using (var query = FacebookQueryFactory.CreateQuery())
                {
                    var dictionary =
                        query.Select("latitude", "longitude").From(FacebookTable.Place).WhereId(id).Execute();
                    if (dictionary == null)
                    {
                        WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound("Business not found");
                        return null;
                    }
                    var geo = new GeoPoint(double.Parse(dictionary["longitude"]), double.Parse(dictionary["latitude"]));
                    using (var dataAccess = new ParkingDataAccess())
                    {
                        parkings = dataAccess.GetParkingsByGeo(geo, 1000, 10).ToList();
                        var business = new Business()
                            {
                                BusinessType = BusinessType.Facebook,
                                FacebookId = id,
                            };
                        
                        parkings.ForEach(business.RecommendedParkings.Add);
                        dataAccess.AddBusiness(business);
                    }
                }
                
            }
            return parkings;
        }

        [WebInvoke(Method = "PUT", UriTemplate = "Parking?id={id}&state={state}")]
        public void ReportStateForParking(string id, string state)
        {
            int identifier;
            if (!int.TryParse(id, out identifier))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            ParkingState currState;
            if (!Enum.TryParse(state, out currState))
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.BadRequest;
            }
            try
            {
                using (var dataAccess = new ParkingDataAccess())
                {
                    // Set the status
                    dataAccess.SetStatusForParking(identifier, currState);
                }
                WebOperationContext.Current.OutgoingResponse.StatusCode = HttpStatusCode.OK;
            }
            catch (KeyNotFoundException)
            {
                // Given parking does not exist
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound("Unable to find parking with the given id");
            }
        }

        #endregion
    }
}
